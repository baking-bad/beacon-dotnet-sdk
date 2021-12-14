// ReSharper disable ArgumentsStyleNamedExpression

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
    using Core.Domain;
    using Netezos.Forging;
    using Netezos.Forging.Models;
    using Netezos.Keys;
    using Netezos.Rpc;
    using Newtonsoft.Json;
    using Serilog.Extensions.Logging;
    using WalletClient;

    public class Sample
    {
        private const string QrCode =
            "BSdNU2tFbvtHv5DMWmY5SHUj1P1DhQ3uH4GrU7x63SpDRWcHaJ7xpshSErQNQ9utuBVxsQ8X1UtGUeZxv6vrLsu6A5k7MAhiUdn5KELPxLP5RiRwJhscwuWNymG93Zf9Pnm1H48K7EwvGqLWsy8J1vpzXgqnDbDKa63baa8DahKcmWJogycFUpDVEtzTtCFMDdXaGGgtx687s28wqFmjoWbwepEkzaqs8fuuBuxmPy4U8N2U3dTxGsrVMXf9Skj9gFg1FQmbtbdaTJp6qzgcHktbUHHWV4MXiC4PQ4Ng3kyvBZgNgYn1vvTa8Kb15VtydMic6cqt";

        public async Task Run()
        {
            const string path = "test1.db";
            File.Delete(path);

            // Existing account in TESTNET
            var walletKey = Key.FromBase58("edsk35n2ruX2r92SdtWzP87mEUqxWSwM14hG6GRhEvU6kfdH8Ut6SW");

            // use this address to receive some tez
            string address = walletKey.PubKey.Address;

            var factory = new WalletBeaconClientFactory();

            var options = new BeaconOptions
            {
                AppName = "Atomex Mobile",
                AppUrl = "", //string?
                IconUrl = "" // string?
            };

            IWalletBeaconClient client = factory.Create(options, new SerilogLoggerFactory());

            client.OnBeaconMessageReceived += async (sender, args) =>
            {
                IBeaconRequest message = args.Request;

                if (message.Type == BeaconMessageType.permission_request)
                {
                    var request = message as PermissionRequest;

                    var response = new PermissionResponse(
                        id: request!.Id,
                        new Network(NetworkType.hangzhounet, "Hangzhounet",
                            "https://hangzhounet.tezblock.io"), // request.Network,
                        scopes: request.Scopes,
                        walletKey.PubKey.ToString());

                    await client.SendResponseAsync(args.SenderId, response);
                }
                else if (message.Type == BeaconMessageType.operation_request)
                {
                    var request = message as OperationRequest;

                    string transactionHash = await MakeTransaction(walletKey);
                    var response = new OperationResponse(
                        id: request!.Id,
                        transactionHash: transactionHash);

                    await client.SendResponseAsync(args.SenderId, response);
                }
            };

            await client.InitAsync();
            client.Connect();

            byte[] decodedBytes = Base58CheckEncoding.Decode(QrCode);
            string message = Encoding.Default.GetString(decodedBytes);

            P2PPairingRequest pairingRequest = JsonConvert.DeserializeObject<P2PPairingRequest>(message);

            await client.AddPeerAsync(pairingRequest!);

            Console.ReadLine();

            client.Disconnect();
        }

        private async Task<string> MakeTransaction(Key key)
        {
            string address = key.PubKey.Address;

            using var rpc = new TezosRpc("https://hangzhounet.api.tez.ie/");

            // get a head block
            string head = await rpc.Blocks.Head.Hash.GetAsync<string>();

            // get account's counter
            int counter = await rpc.Blocks.Head.Context.Contracts[key.Address].Counter.GetAsync<int>();

            var content = new ManagerOperationContent[]
            {
                new TransactionContent
                {
                    Source = address,
                    Counter = ++counter,
                    Amount = 5000000, // 1 tez
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