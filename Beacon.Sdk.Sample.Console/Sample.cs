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
            "BSdNU2tFbvsnyHrxTiB2arfFPW5hvAbPAw2fbWLZnpa2DFGxgdN3CMf4jVmdn4YxNhbeEe8fm7bdv9nxwRYwvEQim4t6VXgjiDRcExAuiiUkSXSn2poNn4hH2kacNyPTSYywqAzMDgtibKFEtp3WPE6JzoCtttfx6LH65sRZj54m4GzgeNtmkd83rMNhXXUkf5FnDhEv16iKGbZj62pG9sdMdWRxADtZCWo41cBPrsRVtbfJWVURBbMzeMLupAoV1pozRUhdCcDWiKPVwhdQgtrZMSnhzngsAygYAojt4PajG4xBrsyH7bECeX2P7uaKVHNjV2LV";

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