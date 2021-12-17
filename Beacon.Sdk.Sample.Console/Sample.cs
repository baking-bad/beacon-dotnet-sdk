// ReSharper disable ArgumentsStyleNamedExpression
// ReSharper disable ArgumentsStyleOther
// ReSharper disable ArgumentsStyleStringLiteral

namespace Beacon.Sdk.Sample.Console
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Base58Check;
    using Beacon;
    using Beacon.Operation;
    using Beacon.Permission;
    using Netezos.Forging;
    using Netezos.Forging.Models;
    using Netezos.Keys;
    using Netezos.Rpc;
    using Newtonsoft.Json;
    using Serilog.Extensions.Logging;
    using WalletBeaconClient;

    public class Sample
    {
        private const string QrCode =
            "BSdNU2tFbvstNHJTvvHbKBzDTC1sPBySNYHr5Ho4whm478TEpuzP1PkG1hpwd9yejWTiKL17nX2p51vWFHzeoQRBdocxXiKtPHh4LcDP2mNJLJdjtcgDW8Jju8M4HMGHtHyezsDR6gZJfbL7sTGo7USsVU3G5Ve18r2N5aNLhvP56ATtZqKsrLGGVixFPrD9b5C8EtSYHM7AFATuRvwhjBLtJRRm3QVdPtVdazDocotDcmE7TtFyk6j4WQjgLHTBqyzBZB2HCAD3S335AicLahNz6S7MYxYvWBDhXChEeYKTKjw2BhYEuZ3kpGQAV9QXTvsHRuj2";

        public async Task Run()
        {
            const string path = "test1.db";
            File.Delete(path);

            // Use existing key
            var walletKey = Key.FromBase58("edsk35n2ruX2r92SdtWzP87mEUqxWSwM14hG6GRhEvU6kfdH8Ut6SW");

            var factory = new WalletBeaconClientFactory();

            var options = new BeaconOptions
            {
                AppName = "Atomex Mobile",
                AppUrl = "", //string?
                IconUrl = "", // string?
                KnownRelayServers = new[] {"beacon-node-0.papers.tech:8448"},
                DatabaseConnectionString = "Filename=test1.db; Mode=Exclusive"
            };

            IWalletBeaconClient walletClient = factory.Create(options, new SerilogLoggerFactory());

            walletClient.OnBeaconMessageReceived += async (_, dAppClient) =>
            {
                BaseBeaconMessage message = dAppClient.Request;

                if (message.Type == BeaconMessageType.permission_request)
                {
                    var request = message as PermissionRequest;

                    var network = new Network(
                        type: NetworkType.hangzhounet,
                        name: "Hangzhounet",
                        rpcUrl: "https://hangzhounet.tezblock.io");

                    var response = new PermissionResponse(
                        id: request!.Id,
                        senderId: walletClient.SenderId,
                        network: network,
                        scopes: request.Scopes,
                        publicKey: walletKey.PubKey.ToString(),
                        appMetadata: walletClient.Metadata);

                    // var response = new BeaconAbortedError(message.Id, walletClient.SenderId);

                    await walletClient.SendResponseAsync(receiverId: dAppClient.SenderId, response);
                }
                else if (message.Type == BeaconMessageType.operation_request)
                {
                    var request = message as OperationRequest;

                    try
                    {
                        string transactionHash = await MakeTransactionAsync(walletKey);

                        var response = new OperationResponse(
                            id: request!.Id,
                            senderId: walletClient.SenderId,
                            transactionHash: transactionHash);

                        await walletClient.SendResponseAsync(receiverId: dAppClient.SenderId, response);
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception.Message);

                        throw;
                    }
                }
            };

            await walletClient.InitAsync();
            walletClient.Connect();

            Console.WriteLine($"client.LoggedIn: {walletClient.LoggedIn}");
            Console.WriteLine($"client.Connected: {walletClient.Connected}");

            byte[] decodedBytes = Base58CheckEncoding.Decode(QrCode);
            string message = Encoding.Default.GetString(decodedBytes);

            P2PPairingRequest pairingRequest = JsonConvert.DeserializeObject<P2PPairingRequest>(message);

            await walletClient.AddPeerAsync(pairingRequest!);

            Console.ReadLine();

            walletClient.Disconnect();
        }

        private static async Task<string> MakeTransactionAsync(Key key)
        {
            using var rpc = new TezosRpc("https://hangzhounet.api.tez.ie/");

            // get a head block
            string head = await rpc.Blocks.Head.Hash.GetAsync<string>();

            // get account's counter
            int counter = await rpc.Blocks.Head.Context.Contracts[key.Address].Counter.GetAsync<int>();

            var content = new ManagerOperationContent[]
            {
                new TransactionContent
                {
                    Source = key.PubKey.Address,
                    Counter = ++counter,
                    Amount = 1000000, // 1 tez
                    Destination = "tz1KhnTgwoRRALBX6vRHRnydDGSBFsWtcJxc",
                    GasLimit = 1500,
                    Fee = 1000 // 0.001 tez
                }
            };

            byte[] bytes = await new LocalForge().ForgeOperationGroupAsync(head, content);

            // sign the operation bytes
            byte[] signature = key.SignOperation(bytes);

            // inject the operation and get its id (operation hash)
            return await rpc.Inject.Operation.PostAsync(bytes.Concat(signature));
        }
    }
}