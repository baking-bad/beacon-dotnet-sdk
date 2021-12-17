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
    using Core.Domain;
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
            "BSdNU2tFbwGhyU9mSkVHF6EsQiyVTTWgseX3jsLZALQq27U4H4jGmYro7gyapirT5pnuJCbXqMQTqnN8KqnfQmFvzyazf48z2xDMfdr5i17WLxGjUhaeG38KwCeVJmxVjD5ZBSC65KLBRVDpWNZ1nqHSiHWQnurpVyq9Sggtd3RoNefE3diNUzXHUicFFboAzujHimsJUJ4M9FEaN7Ca175DD9aNEwWYp896eQgrWuS1s3EL26pcynZWgadajixCHDMAtKCPC5iTaHDGwJaQcaBAPnSfmZN7c14q3LWY7QL5kr44g1e7ZgLPu4Qv7iwhrW9z4aQ9";

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

            IWalletBeaconClient client = factory.Create(options, new SerilogLoggerFactory());

            client.OnBeaconMessageReceived += async (sender, args) =>
            {
                IBeaconRequest message = args.Request;

                if (message.Type == BeaconMessageType.permission_request)
                {
                    var request = message as PermissionRequest;

                    // var network = new Network(
                    //     Type: NetworkType.hangzhounet,
                    //     Name: "Hangzhounet",
                    //     RpcUrl: "https://hangzhounet.tezblock.io");
                    //
                    // var response = new PermissionResponse(
                    //     id: request!.Id,
                    //     network: network,
                    //     scopes: request.Scopes,
                    //     publicKey: walletKey.PubKey.ToString());

                    var response = new BeaconAbortedError(message.Id);

                    await client.SendResponseAsync(args.SenderId, response);
                }
                else if (message.Type == BeaconMessageType.operation_request)
                {
                    var request = message as OperationRequest;

                    try
                    {
                        string transactionHash = await MakeTransactionAsync(walletKey);

                        var response = new OperationResponse(
                            id: request!.Id,
                            transactionHash: transactionHash);

                        await client.SendResponseAsync(args.SenderId, response);
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception.Message);

                        throw;
                    }
                }
            };

            await client.InitAsync();
            client.Connect();

            Console.WriteLine($"client.LoggedIn: {client.LoggedIn}");
            Console.WriteLine($"client.Connected: {client.Connected}");

            byte[] decodedBytes = Base58CheckEncoding.Decode(QrCode);
            string message = Encoding.Default.GetString(decodedBytes);

            P2PPairingRequest pairingRequest = JsonConvert.DeserializeObject<P2PPairingRequest>(message);

            await client.AddPeerAsync(pairingRequest!);

            Console.ReadLine();

            client.Disconnect();
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