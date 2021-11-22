namespace Beacon.Sdk.Sample.Console
{
    using System.Threading.Tasks;
    using Core.Transport.P2P;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Base58Check;
    using Core.Beacon;
    using Core.Transport.P2P.Dto.Handshake;
    using Matrix.Sdk.Core.Domain.Services;
    using Newtonsoft.Json;

    public class Sample
    {
        private readonly ILogger<RelayServerService> _relayServerServiceLogger;
        private readonly ILogger<SessionCryptographyService> _sessionCryptographyServiceLogger;
        private readonly ILogger<PollingService> _pollingServiceLogger;

        public Sample(
            ILogger<RelayServerService> relayServerServiceLogger,
            ILogger<SessionCryptographyService> sessionCryptographyServiceLogger,
            ILogger<PollingService> pollingServiceLogger)
        {
            _relayServerServiceLogger = relayServerServiceLogger;
            _sessionCryptographyServiceLogger = sessionCryptographyServiceLogger;
            _pollingServiceLogger = pollingServiceLogger;
        }

        private const string QrCode = 
                "BSdNU2tFbwG6UthrDFAMvj5f1un83JDsW5xTaXRd3HR8qhdc9EgLHU9hPCQnie5thAHh2obs8JJ79bQMaeTFMGjKy2Y6PsMUNxSeLCFnpqyjWBMkX3LBtJSQVmivJbbJteanbmXfwXa76VkS7hAQthaakkk8dEsKYxHKiqa15JNiX5XBSPDoaLNdKzi2yreYCh3XocTUoNUZC67296H9rViAHhKEajPLf87K68TRSTzvywEuWyd5hznBNZEpDzce5bLSMThtNukxPceGTM4jxKvRr7fusCzPFwh3LvhmpiaUJkSw2rBogs2bTW98kESow955wsK2"
            ;
        
        public async Task Run()
        {
            var factory = new WalletBeaconClientFactory();

            var options = new WalletBeaconClientOptions("Test App Name", null, null);
            IWalletBeaconClient client = factory.Create(options, _relayServerServiceLogger, _sessionCryptographyServiceLogger, _pollingServiceLogger);

            client.OnBeaconMessageReceived += (sender, args) =>
            {
                BeaconBaseMessage beaconBaseMessage = args.BeaconBaseMessage;
            };

            await client.InitAsync();
            client.Connect();

            byte[] decodedBytes = Base58CheckEncoding.Decode(QrCode);
            string message = Encoding.Default.GetString(decodedBytes);
            
            P2PPairingRequest pairingRequest = JsonConvert.DeserializeObject<P2PPairingRequest>(message);
            
            await client.AddPeerAsync(pairingRequest);

            Console.ReadLine();
            
            client.Disconnect();
        }

        public async void Test()
        {
            var factory = new WalletBeaconClientFactory();

            var options = new WalletBeaconClientOptions("Test App Name", null, null);
            IWalletBeaconClient client = factory.Create(options, _relayServerServiceLogger, _sessionCryptographyServiceLogger, _pollingServiceLogger);

            var acknowledgeResponse =
                new AcknowledgeResponse(Constants.BeaconVersion, "test", "senderId");
            await client.RespondAsync(acknowledgeResponse);
        }
    }
}