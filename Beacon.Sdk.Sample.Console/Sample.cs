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
    using Core.Domain;
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
                "BSdNU2tFbwJ8ZaNtN1VsweZceQm1NMinmCWdf5NeKo3WxdJctMqjZABFrgb9aG7nJEa5tQzaDLKzvKZXUabukZhpFhDDEckxCVjK125uxVnjPVZG4XPgNTEgHD5DuqNLDNZaRbk9Jhj5WY4s9PvpUBcHUxHJCJELRi31A8KRmw9nbmepnRjDyiWJiMpKXmQq87w2oCNba2nAvcmggT8qjSsE1VUV3726MKG62kpj6Pz1MRu4HyxqeQ1jBHH9SMuzvhvYzx6KRECa2x8w6vEH2aDGEwQKu9Bi7csmzK43jEgbBpne5raZuBUGvJRzMnrF7t5W4ypX";

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
                IBeaconRequest message = args.Request;
                
                if (message is PermissionRequest request)
                {
                    var response = new PermissionResponse( 
                        id: request!.Id, 
                        network: request.Network, 
                        scopes: request.Scopes, 
                        publicKey: "3b92229274683b311cf8b040cf91ac0f8e19e410f06eda5537ef077e718e0024");
                    
                    client.SendResponseAsync(args.SenderId, response);
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