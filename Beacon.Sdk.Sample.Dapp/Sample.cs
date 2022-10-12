namespace Beacon.Sdk.Sample.Dapp;

using System.Runtime.InteropServices;
using Beacon;
using Beacon.Permission;
using BeaconClients;
using BeaconClients.Abstract;
using Core.Domain.Services;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Extensions.Logging;
using ILogger = Serilog.ILogger;

public class Sample
{
    const string DbPath = "dapp-sample.db";
    private DappBeaconClient BeaconDappClient { get; set; }
    private ILogger Logger { get; set; }

    public async Task Run()
    {
        var options = new BeaconOptions
        {
            AppName = "Dapp sample",
            AppUrl = "https://awesome-dapp.com",
            IconUrl = "https://bcd-static-assets.fra1.digitaloceanspaces.com/dapps/atomex/atomex_logo.jpg",
            KnownRelayServers = Constants.KnownRelayServers,

            DatabaseConnectionString = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? $"Filename={DbPath}; Connection=Shared;"
                : $"Filename={DbPath}; Mode=Exclusive;"
        };

        Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console()
            .CreateLogger();

        ILoggerProvider loggerProvider = new SerilogLoggerProvider(Logger);
        BeaconDappClient = (DappBeaconClient)BeaconClientFactory.Create<IDappBeaconClient>(options, loggerProvider);
        BeaconDappClient.OnBeaconMessageReceived += OnBeaconDappClientMessageReceived;

        await BeaconDappClient.InitAsync();
        BeaconDappClient.Connect();

        string pairingRequestQrData = await BeaconDappClient.GetPairingRequestInfo();
        Logger.Information("Pairing data is Is\n{Data}", pairingRequestQrData);
        var activePeer = BeaconDappClient.GetActivePeer().Result;

        if (activePeer != null)
        {
            var permissions = BeaconDappClient
                .PermissionInfoRepository
                .TryReadBySenderIdAsync(activePeer.SenderId)
                .Result;
            
            var permissionsString = permissions?.Scopes.Aggregate(string.Empty,
                (res, scope) => res + $"{scope}, ") ?? string.Empty;
            
            Logger.Information("We have active peer {Peer} with permissions {Permissions}",
                activePeer.Name, permissionsString);
        }
            
    }

    private async void OnBeaconDappClientMessageReceived(object? sender, BeaconMessageEventArgs e)
    {
        if (e.PairingDone)
        {
            var peer = await BeaconDappClient.GetActivePeer();
            if (peer == null) return;

            var network = new Network
            {
                Type = NetworkType.mainnet,
                Name = "mainnet",
                RpcUrl = "https://rpc.tzkt.io/mainnet"
            };

            var permissionScopes = new List<PermissionScope>
            {
                PermissionScope.operation_request,
                PermissionScope.sign
            };

            var permissionRequest = new PermissionRequest(
                type: BeaconMessageType.permission_request,
                version: Constants.BeaconVersion,
                id: KeyPairService.CreateGuid(),
                senderId: BeaconDappClient.SenderId,
                appMetadata: BeaconDappClient.Metadata,
                network: network,
                scopes: permissionScopes
            );

            await BeaconDappClient.SendResponseAsync(peer.SenderId, permissionRequest);
            return;
        }
        
        var message = e.Request;
        switch (message.Type)
        {
            case BeaconMessageType.permission_response:
            {
                if (message is not PermissionResponse permissionResponse)
                    return;

                var permissionsString = permissionResponse.Scopes.Aggregate(string.Empty,
                    (res, scope) => res + $"{scope}, ");
                
                Logger.Information("{DappName} received permissions {Permissions} from {From} with address {Address} and public key {Pk}",
                    BeaconDappClient.AppName,
                    permissionsString,
                    permissionResponse.AppMetadata.Name,
                    permissionResponse.Address,
                    permissionResponse.PublicKey);
                break;
            };
        }
    }
}