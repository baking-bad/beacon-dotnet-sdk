namespace Beacon.Sdk.Sample.Wallet
{
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;
    using Beacon;
    using Beacon.Error;
    using Beacon.Operation;
    using Beacon.Permission;
    using Beacon.Sign;
    using BeaconClients;
    using BeaconClients.Abstract;
    using Core.Domain.Services;
    using Microsoft.Extensions.Logging;
    using Netezos.Encoding;
    using Netezos.Keys;
    using Serilog;
    using Serilog.Extensions.Logging;
    using ILogger = Serilog.ILogger;

    public class Sample
    {
        private IWalletBeaconClient BeaconWalletClient { get; set; }
        private ILogger Logger { get; set; }
        private static Key TestKey => Key.FromBase58("edsk35muRVNaRkd7ojbHzFdms8E66SADLYyy1enNKYb8k332vCsZ9N");

        public async Task Run()
        {
            const string path = "wallet-beacon-sample.db";

            var options = new BeaconOptions
            {
                AppName = "Wallet sample",
                AppUrl = "https://awesome-wallet.io",
                IconUrl = "https://services.tzkt.io/v1/avatars/KT1TxqZ8QtKvLu3V3JH7Gx58n7Co8pgtpQU5",
                KnownRelayServers = Constants.KnownRelayServers,

                DatabaseConnectionString = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                    ? $"Filename={path}; Connection=Shared;"
                    : $"Filename={path}; Mode=Exclusive;"
            };


            Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .CreateLogger();

            ILoggerProvider loggerProvider = new SerilogLoggerProvider(Logger);
            BeaconWalletClient = BeaconClientFactory.Create<IWalletBeaconClient>(options, loggerProvider);
            BeaconWalletClient.OnBeaconMessageReceived += OnBeaconWalletClientMessageReceived;
            BeaconWalletClient.OnConnectedClientsListChanged += OnConnectedClientsListChanged;

            await BeaconWalletClient.InitAsync();
            BeaconWalletClient.Connect();

            Logger.Information("Paste pairing Qr code here to start pairing with dApp:\n");
            var pairingQrCode = System.Console.ReadLine();

            if (pairingQrCode != null)
            {
                var pairingRequest = BeaconWalletClient.GetPairingRequest(pairingQrCode);
                await BeaconWalletClient.AddPeerAsync(pairingRequest);
            }
        }

        private async void OnBeaconWalletClientMessageReceived(object sender, BeaconMessageEventArgs e)
        {
            var message = e.Request;
            if (message == null) return;

            switch (message.Type)
            {
                case BeaconMessageType.permission_request:
                {
                    if (message is not PermissionRequest permissionRequest)
                        return;

                    var permissionsString = permissionRequest.Scopes.Aggregate(string.Empty,
                        (res, scope) => res + $"{scope}, ");

                    Logger.Information("Permission request received from {Dapp}, requested {Permissions}",
                        permissionRequest.AppMetadata.Name, permissionsString);

                    var response = new PermissionResponse(
                        id: permissionRequest.Id,
                        senderId: BeaconWalletClient.SenderId,
                        appMetadata: BeaconWalletClient.Metadata,
                        network: permissionRequest.Network,
                        scopes: permissionRequest.Scopes,
                        publicKey: TestKey.PubKey.ToString(),
                        version: permissionRequest.Version);

                    await BeaconWalletClient.SendResponseAsync(receiverId: permissionRequest.SenderId, response);
                    break;
                }

                case BeaconMessageType.sign_payload_request:
                {
                    if (message is not SignPayloadRequest signRequest)
                        return;

                    var permissions = BeaconWalletClient
                        .PermissionInfoRepository
                        .TryReadBySenderIdAsync(signRequest.SenderId)
                        .Result;

                    if (permissions == null) return;

                    Logger.Information("Sign payload request received from {Dapp}, payload {Payload}",
                        permissions.AppMetadata.Name, signRequest.Payload);

                    var parsed = Hex.TryParse(signRequest.Payload, out var payloadBytes);

                    if (!parsed)
                    {
                        await BeaconWalletClient.SendResponseAsync(
                            receiverId: signRequest.SenderId,
                            response: new SignatureTypeNotSupportedBeaconError(signRequest.Id,
                                BeaconWalletClient.SenderId));
                        return;
                    }

                    var response = new SignPayloadResponse(
                        signature: TestKey.Sign(payloadBytes),
                        version: signRequest.Version,
                        id: signRequest.Id,
                        senderId: BeaconWalletClient.SenderId);

                    await BeaconWalletClient.SendResponseAsync(receiverId: signRequest.SenderId, response);
                    break;
                }

                case BeaconMessageType.operation_request:
                {
                    if (message is not OperationRequest operationRequest)
                        return;

                    var permissions = BeaconWalletClient
                        .PermissionInfoRepository
                        .TryReadBySenderIdAsync(operationRequest.SenderId)
                        .Result;

                    if (permissions == null) return;

                    Logger.Information("Received operation request from {Dapp}", permissions.AppMetadata.Name);

                    // here you should do Tezos transaction and send response with success transaction hash.
                    // we mock transaction hash here.
                    const string transactionHash = "ooRAfDhmSNiwEdGQi2M5qt27EVtBdh3WD7LX3Rpoet3BTUssKTT";

                    var response = new OperationResponse(
                        id: operationRequest.Id,
                        senderId: BeaconWalletClient.SenderId,
                        transactionHash: transactionHash,
                        operationRequest.Version);

                    await BeaconWalletClient.SendResponseAsync(receiverId: operationRequest.SenderId, response);
                    break;
                }

                default:
                {
                    var error = new BeaconAbortedError(
                        id: KeyPairService.CreateGuid(),
                        senderId: BeaconWalletClient.SenderId);

                    await BeaconWalletClient.SendResponseAsync(receiverId: message.SenderId, error);
                    break;
                }
            }
        }

        private void OnConnectedClientsListChanged(object sender, ConnectedClientsListChangedEventArgs e)
        {
            if (sender is not WalletBeaconClient) return;
            Logger.Information("Connected dApp {Name}", e?.Metadata.Name);
        }
    }
}