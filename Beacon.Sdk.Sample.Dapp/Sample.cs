namespace Beacon.Sdk.Sample.Dapp;

using System.Runtime.InteropServices;
using Beacon;
using Beacon.Operation;
using Beacon.Permission;
using Beacon.Sign;
using BeaconClients;
using BeaconClients.Abstract;
using Core.Domain.Services;
using Microsoft.Extensions.Logging;
using Netezos.Keys;
using Newtonsoft.Json.Linq;
using Serilog;
using Serilog.Extensions.Logging;
using Hex = Netezos.Encoding.Hex;
using ILogger = Serilog.ILogger;

public class Sample
{
    const string DbPath = "dapp-sample.db";
    private const string TzButtonColorsContract = "KT1RPW5kTX6WFxg8JK34rGEU24gqEEudyfvz";
    private const string TokenId = "925";

    private const string PayloadToSign =
        "05010000008654657a6f73205369676e6564204d6573736167653a20436f6e6669726d696e67206d79206964656e7469747920617320747a31524445344a64556f37336278323363776a72393767446b6350363362344e664744206f6e206f626a6b742e636f6d2c207369673a6f5252764f6374513638726463457555394965782d72496b45516d46426652";

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
        var activePeer = await BeaconDappClient.GetActivePeer();
        if (activePeer == null) return;

        var permissions = await BeaconDappClient
            .PermissionInfoRepository
            .TryReadBySenderIdAsync(activePeer.SenderId);
        if (permissions == null) return;

        var permissionsString = permissions?.Scopes.Aggregate(string.Empty,
            (res, scope) => res + $"{scope}, ") ?? string.Empty;

        var pubKey = PubKey.FromBase58(permissions!.PublicKey);

        Logger.Information("We have active peer {Peer} with permissions {Permissions} and address {Address}",
            activePeer.Name, permissionsString, pubKey.Address);

        var requestCommand = true;

        while (requestCommand)
        {
            var command = Console.ReadLine();

            switch (command)
            {
                case "exit":
                {
                    requestCommand = false;
                    break;
                }
                case "sign":
                {
                    var signPayloadRequest = new SignPayloadRequest(
                        id: KeyPairService.CreateGuid(),
                        version: Constants.BeaconVersion,
                        senderId: BeaconDappClient.SenderId,
                        signingType: SignPayloadType.raw,
                        payload: PayloadToSign,
                        sourceAddress: pubKey.Address);

                    await BeaconDappClient.SendResponseAsync(activePeer.SenderId, signPayloadRequest);
                    break;
                }
                case "operation":
                {
                    var operationDetails = new List<PartialTezosTransactionOperation>
                    {
                        new(
                            Amount: "0",
                            Destination: TzButtonColorsContract,
                            Parameters: new JObject
                            {
                                ["entrypoint"] = "set_color",
                                ["value"] = new JObject
                                {
                                    ["int"] = TokenId
                                }
                            })
                    };

                    var operationRequest = new OperationRequest(
                        type: BeaconMessageType.operation_request,
                        version: Constants.BeaconVersion,
                        id: KeyPairService.CreateGuid(),
                        senderId: BeaconDappClient.SenderId,
                        network: permissions.Network,
                        operationDetails: operationDetails,
                        sourceAddress: pubKey.Address);

                    await BeaconDappClient.SendResponseAsync(activePeer.SenderId, operationRequest);
                    break;
                }
            }
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

                Logger.Information(
                    "{DappName} received permissions {Permissions} from {From} with address {Address} and public key {Pk}",
                    BeaconDappClient.AppName,
                    permissionsString,
                    permissionResponse.AppMetadata.Name,
                    permissionResponse.Address,
                    permissionResponse.PublicKey);
                break;
            }

            case BeaconMessageType.operation_response:
            {
                if (message is not OperationResponse operationResponse)
                    return;

                Logger.Debug("Operation completed with transaction hash {Tx}", operationResponse.TransactionHash);
                break;
            }

            case BeaconMessageType.sign_payload_response:
            {
                if (message is not SignPayloadResponse signPayloadResponse)
                    return;

                var senderPermissions = await BeaconDappClient
                    .PermissionInfoRepository
                    .TryReadBySenderIdAsync(signPayloadResponse.SenderId);
                if (senderPermissions == null) return;

                var pubKey = PubKey.FromBase58(senderPermissions.PublicKey);
                var payloadBytes = Hex.Parse(PayloadToSign);
                var verified = pubKey.Verify(payloadBytes, signPayloadResponse.Signature);
                var stringVerifyResult = verified ? "Successfully" : "Unsuccessfully";

                Logger.Information("{Result} signed payload by {Sender}, signature is {Signature}",
                    stringVerifyResult, senderPermissions.AppMetadata.Name, signPayloadResponse.Signature);
                break;
            }
        }
    }
}