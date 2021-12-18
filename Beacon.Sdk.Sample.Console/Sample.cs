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
    using Beacon.Error;
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
            "BSdNU2tFbvqMEHQk8NN847VqF3dWV364n25KmmBWeExZJH3F1mUo4EbcZ8hC2VojBQ6iaX6x7p9qvfJt8WCLSpu4gSt7pM1AAEK6ynHSYMtPYGJs3H6VxoGKPvk4MmgnSrVZvwSoTFCqS7AxexYLAfZpvYw2qgwZrHmfnVMzvNds46PDTjoxnF7kBjbWsxnCTArWFSxpaDXFxFzLWzN2gRoVRRzJnjEWPByBadYpDWk5EaB8ZH5tWH7baUwrNLA6ZA47898gWQeKCZUyh4nAk4tuZhNMXbQXnGnZLs2Ac5Ny7fXdk52Z3t9sTbVM46rRBNHBrinJ";

        public async Task Run()
        {
            const string path = "test1.db";
            // const string path = "prod_test.db";
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
                // see https://github.com/mbdavid/LiteDB/issues/787
                DatabaseConnectionString = $"Filename={path}; Mode=Exclusive", // mac m1
                // DatabaseConnectionString = $"Filename={path}"
            };

            IWalletBeaconClient walletClient = factory.Create(options);

            walletClient.OnBeaconMessageReceived += async (_, dAppClient) =>
            {
                BaseBeaconMessage message = dAppClient.Request;

                if (message.Type == BeaconMessageType.permission_request)
                {
                    var request = message as PermissionRequest;
                    
                    var network = new Network 
                    {
                        Type = NetworkType.hangzhounet,
                        Name = "Hangzhounet",
                        RpcUrl = "https://hangzhounet.tezblock.io"
                    };

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
                    
                    string transactionHash = await MakeTransactionAsync(walletKey);

                    var response = new OperationResponse(
                        id: request!.Id,
                        senderId: walletClient.SenderId,
                        transactionHash: transactionHash);

                    await walletClient.SendResponseAsync(receiverId: dAppClient.SenderId, response);
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