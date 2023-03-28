namespace Beacon.Sdk.Sample.Dapp;

using System.Runtime.InteropServices;
using Beacon;
using Beacon.Error;
using Beacon.Operation;
using Beacon.Permission;
using Beacon.Sign;
using BeaconClients;
using BeaconClients.Abstract;
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

    private const string PayloadToSign =
        "05010000008654657a6f73205369676e6564204d6573736167653a20436f6e6669726d696e67206d79206964656e7469747920617320747a31524445344a64556f37336278323363776a72393767446b6350363362344e664744206f6e206f626a6b742e636f6d2c207369673a6f5252764f6374513638726463457555394965782d72496b45516d46426652";

    private IDappBeaconClient BeaconDappClient { get; set; }
    private ILogger Logger { get; set; }

    public async Task Run()
    {
        var options = new BeaconOptions
        {
            AppName = "Dapp sample",
            AppUrl = "https://awesome-dapp.io",
            IconUrl = "https://services.tzkt.io/v1/avatars/KT1TxqZ8QtKvLu3V3JH7Gx58n7Co8pgtpQU5",
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
        BeaconDappClient = BeaconClientFactory.Create<IDappBeaconClient>(options, loggerProvider);
        BeaconDappClient.OnBeaconMessageReceived += OnBeaconDappClientMessageReceived;
        BeaconDappClient.OnConnectedClientsListChanged += OnConnectedClientsListChanged;

        await BeaconDappClient.InitAsync();
        BeaconDappClient.Connect();

        var pairingRequestQrData = BeaconDappClient.GetPairingRequestInfo();
        Logger.Information("Pairing data is\n{Data}", pairingRequestQrData);

        var requestCommand = true;
        while (requestCommand)
        {
            var activeAccountPermissions = BeaconDappClient.GetActiveAccount();
            if (activeAccountPermissions == null)
            {
                Logger.Information("We don't have active account, you can pair this dApp with some wallet");
                Console.ReadLine();
                return;
            }

            var pubKey = PubKey.FromBase58(activeAccountPermissions.PublicKey);
            var permissionsString = activeAccountPermissions.Scopes.Aggregate(string.Empty,
                (res, scope) => res + $"{scope}, ");
            Logger.Information("We have active account {Address} in wallet {Wallet} with permissions {Permissions}",
                pubKey.Address, activeAccountPermissions.AppMetadata.Name, permissionsString);

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
                    await BeaconDappClient.RequestSign(PayloadToSign, SignPayloadType.raw);
                    break;
                }
                case "operation":
                {
                    const string stringParams = @"{
	                    'entrypoint': 'login',
	                        'value': {
		                        'prim': 'Unit'
	                        }
                        }";

                    var operationDetails = new List<PartialTezosTransactionOperation>
                    {
                        new(
                            Amount: "0",
                            Destination: "KT1WguzxyLmuKbJhz3jNuoRzzaUCncfp6PFE",
                            Parameters: JObject.Parse(stringParams))
                    };

                    await BeaconDappClient.RequestOperation(operationDetails);
                    break;
                }
                case "logout":
                {
                    BeaconDappClient.RemoveActiveAccounts();
                    break;
                }
            }
        }
    }

    private void OnConnectedClientsListChanged(object? sender, ConnectedClientsListChangedEventArgs? e)
    {
        if (sender is not DappBeaconClient) return;
        Logger.Information("Connected wallet {Name}", e?.Metadata.Name);
    }

    private async void OnBeaconDappClientMessageReceived(object? sender, BeaconMessageEventArgs e)
    {
        if (e.PairingDone)
        {
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

            await BeaconDappClient.RequestPermissions(permissionScopes, network);
            return;
        }

        var message = e.Request;
        if (message == null) return;

        var senderPermissions = await BeaconDappClient
            .PermissionInfoRepository
            .TryReadBySenderIdAsync(message.SenderId);
        if (senderPermissions == null) return;

        switch (message.Type)
        {
            case BeaconMessageType.permission_response:
            {
                if (message is not PermissionResponse)
                    return;

                var permissionsString = senderPermissions.Scopes.Aggregate(string.Empty,
                    (res, scope) => res + $"{scope}, ");

                Logger.Information(
                    "{DappName} received permissions {Permissions} from {From} with address {Address} and public key {Pk}",
                    BeaconDappClient.AppName,
                    permissionsString,
                    senderPermissions.AppMetadata.Name,
                    senderPermissions.Address,
                    senderPermissions.PublicKey);
                break;
            }

            case BeaconMessageType.operation_response:
            {
                if (message is not OperationResponse operationResponse)
                    return;

                Logger.Information("Operation completed with transaction hash {Tx}", operationResponse.TransactionHash);
                break;
            }

            case BeaconMessageType.sign_payload_response:
            {
                if (message is not SignPayloadResponse signPayloadResponse)
                    return;

                var pubKey = PubKey.FromBase58(senderPermissions.PublicKey);
                var payloadBytes = Hex.Parse(PayloadToSign);
                var verified = pubKey.Verify(payloadBytes, signPayloadResponse.Signature);
                var stringVerifyResult = verified ? "Successfully" : "Unsuccessfully";

                Logger.Information("{Result} signed payload by {Sender}, signature is {Signature}",
                    stringVerifyResult, senderPermissions.AppMetadata.Name, signPayloadResponse.Signature);
                break;
            }

            case BeaconMessageType.error:
            {
                if (message is not BaseBeaconError baseBeaconError)
                    return;

                Logger.Error("Received error with type {Type} response from {From}",
                    baseBeaconError.ErrorType, senderPermissions.AppMetadata.Name);
                break;
            }
        }
    }
}