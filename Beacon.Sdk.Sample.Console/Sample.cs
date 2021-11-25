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
                "BSdNU2tFbwJ8gKbPkp7ui1RRLDkTaMVm9ngbP974VJbn7KXzzCDpzG7GqFHkqtuv1LY3KCxEtuJ4usTVdPN4xgE47NnTUP7nhPVCTkT2TVByj2FVnXYNZpBfTwGBXX9NixPMbKzfNp9jceYLxM3eFpJi5LotsSMbdM4vUv2eNRARwcCGS2REKftwd4nEB1nBrkhUYMhe1rPtaABdfd4Zk8WY53EJaMtHSgv8qHoqaszLaxgPy6PocEDfgP6oHAnsUgY8ncAEmqLqeGrcK4i4VnzxVGR56N1jiWmQGFiq4DSbCjGFLys4uGda9okzyM9rURXWnPYy"
            ;

        public async Task Run()
        {
            const string path = "test1.db";
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