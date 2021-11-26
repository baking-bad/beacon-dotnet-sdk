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
                "BSdNU2tFbvpkRbsrwoi67AWq14RZVWFWf9GZ3616m2vhnAx6WZVPC6uZ2d8ekZ8WekY1VC52b5oNcJfZCgicKkQ7URc4ER7KLKiuDtcoCT8RkorBcWEsZweqJkDDaBzavNM8n29DCARRK1fKaRhERQgdMfzxx5KYRqJK5qXzL7XGBR7UGFhWh8UobMSq8Eg982AwPGdUgfPHfZZUsBez2YCBUuiySTSmDo4gSt6t68F5auzTNsNfLNCiqcemoCkYTShFFNTpXcCjqN52ywkMsHh5pM68QiWhFfhBoYBfnRRcDE4zbqdGrW24FB92suQnq7ZkMeuz";

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
                BeaconBaseMessage beaconBaseMessage = args.BeaconBaseMessage;

                if (beaconBaseMessage.Type == BeaconMessageType.permission_request)
                {
                    var response = new PermissionResponse()
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