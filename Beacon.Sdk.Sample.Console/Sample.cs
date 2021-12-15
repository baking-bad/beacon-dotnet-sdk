// ReSharper disable ArgumentsStyleNamedExpression

namespace Beacon.Sdk.Sample.Console
{
    using System;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using Base58Check;
    using Beacon;
    using Beacon.Operation;
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
            "BSdNU2tFbvrGQoe9zMh8XXYEf3geBM237D9WVrsCxPSPctGMfCznJByJyoinpNcU6AQWasVADB7WwWBuS8kJtzHASXGF8n69A7xLvcU2HiR7ZqZx6xQUdyVkGFVTWYgd9HKwDW2bfr3veddXPqfkc2LtxK5jUcMKebDsGRM19TdxE1gkpujDf19vpu1syAVkdzSHqxAJMwnTiMYQ22UusCq6YctJkwFpiPyoms3fSCv298ntN5A9QkHxbNVDLZqnB3quKhUDW8sWECtVrxWEuSDTosmUaHwWefQQ7pDprj1gX44Kt3qoGeRne4FVkeFqQnK8hER5";

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

                if (message.Type == BeaconMessageType.permission_request)
                {
                    var request = message as PermissionRequest;
                    
                    var response = new PermissionResponse(
                        id: request!.Id,
                        network: request.Network,
                        scopes: request.Scopes,
                        "3b92229274683b311cf8b040cf91ac0f8e19e410f06eda5537ef077e718e0024");

                    client.SendResponseAsync(args.SenderId, response);
                }

                if (message.Type == BeaconMessageType.operation_request)
                {
                    var request = message as OperationRequest;
                    
                    
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
            var seedRepository =
                new LiteDbSeedRepository(new Logger<LiteDbSeedRepository>(loggerFactory), repositorySettings);

            var keyPairService = new KeyPairService(cryptographyService, seedRepository);

            var guid = Guid.NewGuid().ToString();
            string g = KeyPairService.CreateGuid();
        }
    }
}