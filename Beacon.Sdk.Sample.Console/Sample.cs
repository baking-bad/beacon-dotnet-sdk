// ReSharper disable ArgumentsStyleNamedExpression
namespace Beacon.Sdk.Sample.Console
{
    using System;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using Base58Check;
    using Beacon;
    using Beacon.Permission;
    using Core.Domain.Services;
    using Core.Infrastructure.Cryptography;
    using Core.Infrastructure.Repositories;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using Serilog.Extensions.Logging;
    using WalletClient;

    public class Sample
    {
        private const string QrCode = 
                "BSdNU2tFbwJ8DZffivKRbRkKSQo81QRMdoUn32MZVgwYBWUrYZHhGDWP2fUrLsSBrD4YxNKchwjrsLwTJJB9Kt3iNzBoqPhcM8S8WFcueydCghDeiHvVo98qhpqjUKny5bCzcdsJTcKLLQCVUpjCdZ6mek9RDX2z3u7atRn8UNG2amibHB9HoPBsWRvrCrjLRSnRQLquKwLUBJxHErkFS2Yn2TR2SbKWNSMYDAjNrrTPeMtLxauDSRNbWm5Uxu45dKDYC4cBpCStSUCcmUnCbVwxgBHxjiCZEFKxLpo8Cx66R3CrP6CC6aRKZdGw3CoHvKzHdHjm";

        public async Task Run()
        {
            const string path = "test1.db";
            File.Delete(path);
            
            var factory = new WalletClientFactory();

            var options = new BeaconOptions
            {
                AppName = "Atomex Mobile",
                AppUrl = "", //string?
                IconUrl = "" // string?
            };
    
            IWalletClient client = factory.Create(options, new SerilogLoggerFactory());

            client.OnBeaconMessageReceived += (sender, args) =>
            {
                BeaconBaseMessage message = args.BeaconBaseMessage;
                
                if (message is PermissionRequest request)
                {
                    var response = new PermissionResponse( 
                        id: request!.Id, 
                        network: request.Network, 
                        scopes: request.Scopes, 
                        publicKey: "3b92229274683b311cf8b040cf91ac0f8e19e410f06eda5537ef077e718e0024");
                    
                    client.SendMessageAsync(args.SenderId, response);
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

        public void TestRepositories()
        {
            var path = "test.db";
            File.Delete(path);
            var repositorySettings = new RepositorySettings
            {
                ConnectionString = path
            };
            
            var cryptographyService = new CryptographyService();
            var loggerFactory = new SerilogLoggerFactory();
            var seedRepository = new LiteDbSeedRepository(new Logger<LiteDbSeedRepository>(loggerFactory), repositorySettings);
            
            var keyPairService = new KeyPairService(cryptographyService, seedRepository);

            var guid = Guid.NewGuid().ToString();
            var g = KeyPairService.CreateGuid();
        }
    }
}

// string path = "test.db";
// File.Delete(path);
// var repositorySettings = new RepositorySettings
// {
//     ConnectionString = path
// };
// var loggerFactory = new SerilogLoggerFactory();
// var seedRepository = new LiteDbSeedRepository(new Logger<LiteDbSeedRepository>(loggerFactory), repositorySettings);
//
// var generator = RandomNumberGenerator.Create();
//
// var bytes = new byte[SeedBytes]; 
// generator.GetBytes(bytes);
//
// // KeyPairService
//
// string seed = seedRepository.Create(Guid.NewGuid().ToString()).Result;
// seed = seedRepository.TryRead().Result;
//
// // seed = seedRepository.Create(Guid.NewGuid().ToString()).Result;
// seed = seedRepository.TryRead().Result;