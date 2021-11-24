namespace Beacon.Sdk.Sample.Console
{
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using System;
    using System.IO;
    using System.Text;
    using Base58Check;
    using Beacon;
    using Core.Domain.P2P;
    using Core.Domain.Services;
    using Core.Infrastructure.Cryptography;
    using Core.Infrastructure.Repositories;
    using Matrix.Sdk.Core.Domain.Services;
    using Newtonsoft.Json;
    using Serilog.Extensions.Logging;
    using WalletClient;

    public class Sample
    {
        private readonly ILogger<P2PLoginRequestFactory> _relayServerServiceLogger;
        private readonly ILogger<P2PMessageService> _sessionCryptographyServiceLogger;
        private readonly ILogger<PollingService> _pollingServiceLogger;

        public Sample(
            ILogger<P2PLoginRequestFactory> relayServerServiceLogger,
            ILogger<P2PMessageService> sessionCryptographyServiceLogger,
            ILogger<PollingService> pollingServiceLogger)
        {
            _relayServerServiceLogger = relayServerServiceLogger;
            _sessionCryptographyServiceLogger = sessionCryptographyServiceLogger;
            _pollingServiceLogger = pollingServiceLogger;
        }

        private const string QrCode = 
                "BSdNU2tFbvqrkNr3R7P5gccbtDbC1b6iAxT1x3VDKhEWit6PRS8mHc3AqxsDx6a8SbExWNB1FsCXJWe8EQ8rfBi1GtVtuPFeYg4xfYJu8xn1PvriUHxqcNHHoxhRLmxeAHWkMRwQAseA2zesF18acQ1wAQm9SCCJ2NCe4NQRfB1Fg2B2rkF1rbHYtT3BQkoW5SrJitfqaKGRzWX9LmaR8QcLHDP8iCv6bnacpq9oo6KwwvmdNi5XL44pseRWub1uBDSr2g4tGFC4MR7RYFmfe224nh5LKndia55ZM1KKQZVmuAc4RtjSqZnFpcoXHgJi5zga8SuW"
            ;

        public async Task Run()
        {
            const string path = "test.db";
            File.Delete(path);
            
            var factory = new WalletClientFactory();

            var options = new WalletClientOptions
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