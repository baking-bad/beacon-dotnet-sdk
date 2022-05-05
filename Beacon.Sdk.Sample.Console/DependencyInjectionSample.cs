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
    using Microsoft.Extensions.DependencyInjection;
    using Netezos.Forging;
    using Netezos.Forging.Models;
    using Netezos.Keys;
    using Netezos.Rpc;
    using Newtonsoft.Json;

    public class DependencyInjectionSample
    {
        public async Task Run(IServiceProvider serviceProvider)
        {
            Console.WriteLine("Enter QR code:");
            string qrCode = Console.ReadLine();
            
            const string path = "test1.db";
            // const string path = "prod_test.db";
            File.Delete(path);
            
            // Use existing key
            var walletKey = Key.FromBase58("edsk35n2ruX2r92SdtWzP87mEUqxWSwM14hG6GRhEvU6kfdH8Ut6SW");
            
            IWalletBeaconClient walletClient = serviceProvider.GetRequiredService<IWalletBeaconClient>();
            
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

                    var publicKey = PubKey.FromBase58(walletKey.PubKey.ToString());
                    string address = publicKey.Address;
                    
                    // var u = request
                    var response = new PermissionResponse(
                        id: request!.Id,
                        senderId: walletClient.SenderId,
                        network: network,
                        scopes: request.Scopes,
                        publicKey: publicKey.ToString(),
                        address: address,
                        appMetadata: walletClient.Metadata);

                    // var response = new BeaconAbortedError(message.Id, walletClient.SenderId);

                    await walletClient.SendResponseAsync(receiverId: dAppClient.SenderId, response);
                }
                else if (message.Type == BeaconMessageType.operation_request)
                {
                    var request = message as OperationRequest;

                    if (request!.OperationDetails.Count <= 0) return;
                    
                    PartialTezosTransactionOperation operation = request.OperationDetails[0];
                    if (long.TryParse(operation.Amount, out long amount))
                    {
                        string transactionHash =
                            await MakeTransactionAsync(walletKey, operation.Destination, amount);

                        var response = new OperationResponse(
                            id: request!.Id,
                            senderId: walletClient.SenderId,
                            transactionHash: transactionHash);

                        await walletClient.SendResponseAsync(receiverId: dAppClient.SenderId, response);   
                    }
                }
            };

            await walletClient.InitAsync();
            walletClient.Connect();

            Console.WriteLine($"client.LoggedIn: {walletClient.LoggedIn}");
            Console.WriteLine($"client.Connected: {walletClient.Connected}");

            byte[] decodedBytes = Base58CheckEncoding.Decode(qrCode);
            string message = Encoding.Default.GetString(decodedBytes);
            
            P2PPairingRequest pairingRequest = JsonConvert.DeserializeObject<P2PPairingRequest>(message);

            await walletClient.AddPeerAsync(pairingRequest!);

            Console.ReadLine();

            walletClient.Disconnect();
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