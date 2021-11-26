namespace Beacon.Sdk.Sample.Console
{
    using System;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using Base58Check;
    using Beacon;
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
                "BSdNU2tFbwHcZchafP24HAnC3enY5bMwP2o8b2hSZFEEEs1ur9pTKEbzmwbTHt7GVZH46yod8pY8P2BHNvUMprPFkqodpZVQz3EeKY9byWAbdxvkBKTXySRSFEPmuGpg5CJMw99UuhKJHCb1s2KfuUSBpq29EADrrho3KGqcyqdktkCzdKZodEpcmmCe7joAZsgDRM9tHcU3ijuG4yUcSnofLjGZA2QdKuToGVWcQZv6PtRjyhAj5tHN1doT463TLXhPYFeT6vFzmfqiRvMByvGU1e29CZuqH1TeWpdyXBkKXFgGc7kemhPFNMURGsxH418k77vL";

        public async Task Run()
        {
            const string path = "test1.db";
            File.Delete(path);
            
            var factory = new WalletClientFactory();

            var options = new ClientOptions
            {
                AppName = "Atomex Mobile",
                AppUrl = "", //string?
                IconUrl = "" // string?
            };
    
            IWalletClient client = factory.Create(options, new SerilogLoggerFactory());

            client.OnBeaconMessageReceived += (sender, args) =>
            {
                BeaconBaseMessage beaconBaseMessage = args.BeaconBaseMessage;
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