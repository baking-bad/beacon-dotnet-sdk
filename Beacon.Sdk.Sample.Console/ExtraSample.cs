namespace Beacon.Sdk.Sample.Console
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Domain.Services;
    using Core.Infrastructure.Cryptography;
    using Core.Infrastructure.Repositories;
    using Microsoft.Extensions.Logging;
    using Netezos.Forging;
    using Netezos.Forging.Models;
    using Netezos.Keys;
    using Netezos.Rpc;
    using Serilog.Extensions.Logging;

    public class ExtraSample
    {
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

        public async Task TestTransaction()
        {
            // or use existing one
            var key = Key.FromBase58("edsk35n2ruX2r92SdtWzP87mEUqxWSwM14hG6GRhEvU6kfdH8Ut6SW");

            // use this address to receive some tez
            string address = key.PubKey.Address;

            // using var rpc = new TezosRpc("https://mainnet-tezos.giganode.io/");
            using var rpc = new TezosRpc("https://hangzhounet.api.tez.ie/");

            // get a head block
            string head = await rpc.Blocks.Head.Hash.GetAsync<string>();

            // get account's counter
            int counter = await rpc.Blocks.Head.Context.Contracts[key.Address].Counter.GetAsync<int>();

            var content = new ManagerOperationContent[]
            {
                // new RevealContent
                // {
                //     Source = address,
                //     Counter = ++counter,
                //     PublicKey = key.PubKey.GetBase58(),
                //     GasLimit = 1500,
                //     Fee = 1000 // 0.001 tez
                // },
                new TransactionContent
                {
                    Source = address,
                    Counter = ++counter,
                    Amount = 1000000, // 1 tez
                    Destination = "tz1KhnTgwoRRALBX6vRHRnydDGSBFsWtcJxc",
                    GasLimit = 1500,
                    Fee = 1000 // 0.001 tez
                }
            };

            byte[] bytes = await new LocalForge().ForgeOperationGroupAsync(head, content);

            // sign the operation bytes
            byte[] signature = key.SignOperation(bytes);

            // inject the operation and get its id (operation hash)
            dynamic result = await rpc.Inject.Operation.PostAsync(bytes.Concat(signature));
        }

        public async void TestPublicKey()
        {
            // var accountService = new AccountService(new CryptographyService());
            // var key = new Key();
            // string pubKey = key.PubKey.ToString();

            // var address = key.PubKey.Address;
            // var k = Key.FromBase58(key.PubKey);

            // var k = Encoding.Default.GetBytes(pubKey);// Hex.Convert(pubKey);
            // var key1 = PubKey.FromBase58(pubKey);
            // var pK = key1.ToString();
        }
    }
}