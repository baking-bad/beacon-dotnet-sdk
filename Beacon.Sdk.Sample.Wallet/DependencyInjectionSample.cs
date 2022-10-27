using Serilog;

namespace Beacon.Sdk.Sample.Console
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Beacon;
    using Beacon.Operation;
    using Beacon.Permission;
    using BeaconClients.Abstract;
    using Microsoft.Extensions.DependencyInjection;
    using Netezos.Encoding;
    using Netezos.Forging;
    using Netezos.Forging.Models;
    using Netezos.Keys;
    using Netezos.Rpc;
    using Newtonsoft.Json;

    public class DependencyInjectionSample
    {
        public async Task Run(IServiceProvider serviceProvider)
        {
            // Log.Information("Enter QR code:");
            //
            // var qrCode = Console.ReadLine();
            //
            // // Use existing key
            // var walletKey = Key.FromBase58("");
            //
            // var walletClient = serviceProvider.GetRequiredService<IWalletBeaconClient>();
            //
            // walletClient.OnBeaconMessageReceived += async (_, dAppClient) =>
            // {
            //     var message = dAppClient.Request;
            //
            //     Log.Information("Handling message with id {@MessageId} Type {MessageType}",
            //         message.Id,
            //         message.Type.ToString());
            //
            //     switch (message.Type)
            //     {
            //         case BeaconMessageType.permission_request:
            //         {
            //             var request = message as PermissionRequest;
            //
            //             var network = request!.Network.Type switch
            //             {
            //                 NetworkType.mainnet => new Network
            //                 {
            //                     Type = NetworkType.mainnet,
            //                     Name = "Mainnet",
            //                     RpcUrl = "https://rpc.tzkt.io/mainnet"
            //                 },
            //                 _ => new Network
            //                 {
            //                     Type = NetworkType.ithacanet,
            //                     Name = "Ithacanet",
            //                     RpcUrl = "https://rpc.tzkt.io/ithacanet"
            //                 }
            //             };
            //
            //             // change response sign to encrypt permission
            //             var scopes = request.Scopes
            //                 .Select(s => s == PermissionScope.sign ? PermissionScope.encrypt : s)
            //                 .ToList();
            //
            //             var publicKey = PubKey.FromBase58(walletKey.PubKey.ToString());
            //
            //             var response = new PermissionResponse(
            //                 id: request!.Id,
            //                 senderId: walletClient.SenderId,
            //                 appMetadata: walletClient.Metadata,
            //                 network: network,
            //                 scopes: scopes,
            //                 publicKey: publicKey.ToString(),
            //                 version: request.Version);
            //
            //             // var response = new BeaconAbortedError(message.Id, walletClient.SenderId);
            //
            //             await walletClient.SendResponseAsync(receiverId: dAppClient.SenderId, response);
            //             break;
            //         }
            //         case BeaconMessageType.operation_request:
            //         {
            //             var request = message as OperationRequest;
            //
            //             // if (request!.OperationDetails.Count <= 0) return;
            //
            //             PartialTezosTransactionOperation operation = request.OperationDetails[0];
            //
            //             if (long.TryParse(operation?.Amount, out long amount))
            //             {
            //                 // string transactionHash =
            //                 //     await MakeTransactionAsync(walletKey, operation.Destination, amount);
            //
            //                 var response = new OperationResponse(
            //                     id: request!.Id,
            //                     senderId: walletClient.SenderId,
            //                     transactionHash: "txHash",
            //                     request.Version);
            //
            //                 await walletClient.SendResponseAsync(receiverId: dAppClient.SenderId, response);
            //             }
            //
            //             break;
            //         }
            //         case BeaconMessageType.sign_payload_request:
            //             break;
            //         case BeaconMessageType.broadcast_request:
            //             break;
            //         case BeaconMessageType.permission_response:
            //             break;
            //         case BeaconMessageType.sign_payload_response:
            //             break;
            //         case BeaconMessageType.operation_response:
            //             break;
            //         case BeaconMessageType.broadcast_response:
            //             break;
            //         case BeaconMessageType.acknowledge:
            //             break;
            //         case BeaconMessageType.disconnect:
            //             break;
            //         case BeaconMessageType.error:
            //             break;
            //         default:
            //             throw new ArgumentOutOfRangeException();
            //     }
            // };
            //
            // await walletClient.InitAsync();
            // walletClient.Connect();
            //
            // Console.WriteLine($"client.LoggedIn: {walletClient.LoggedIn}");
            // Console.WriteLine($"client.Connected: {walletClient.Connected}");
            //
            // byte[] decodedBytes = Base58.Parse(qrCode);
            // string message = Encoding.Default.GetString(decodedBytes);
            //
            // P2PPairingRequest pairingRequest = JsonConvert.DeserializeObject<P2PPairingRequest>(message);
            //
            // string pairingString = JsonConvert.SerializeObject(pairingRequest);
            // byte[] pairingBytes = Encoding.UTF8.GetBytes(pairingString);
            //
            // // await walletClient.AddPeerAsync(pairingRequest!);
            //
            // Console.ReadLine();
            // walletClient.Disconnect();
        }

        private static async Task<string> MakeTransactionAsync(Key key, string destination, long amount)
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
                    Amount = amount,
                    Destination = destination,
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