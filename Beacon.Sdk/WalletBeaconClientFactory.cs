namespace Beacon.Sdk
{
    using Core.Infrastructure.Cryptography;
    using Core.Infrastructure.Repositories;
    using Core.Infrastructure.Serialization;
    using Core.Transport.P2P;
    using Core.Transport.P2P.ChannelOpening;
    using Matrix.Sdk;
    using Matrix.Sdk.Core.Domain.Services;
    using Matrix.Sdk.Core.Infrastructure.Services;
    using Microsoft.Extensions.Logging;

    public class WalletBeaconClientFactory
    {
        private static readonly MatrixClientFactory MatrixClientFactory = new();
        private static readonly SingletonHttpFactory SingletonHttpFactory = new();
        private static readonly CryptographyService CryptographyService = new();
        
        private IWalletBeaconClient? _client;
        
        public IWalletBeaconClient Create(
            WalletBeaconClientOptions options, 
            ILogger<RelayServerService>? relayServerServiceLogger,
            ILogger<SessionCryptographyService> sessionCryptographyServiceLogger, 
            ILogger<PollingService> pollingServiceLogger)
        {
            if (_client != null)
                return _client;

            var keyPairRepository = new InMemoryKeyPairRepository(CryptographyService);
            var beaconPeerRepository = new InMemoryBeaconPeerRepository(CryptographyService);

            var clientService = new ClientService(SingletonHttpFactory);
            var sdkStorage = new SdkStorage();
            var relayServerService = new RelayServerService(relayServerServiceLogger, clientService, sdkStorage);
            
            var sessionCryptographyService = new SessionCryptographyService(
                keyPairRepository, 
                beaconPeerRepository, 
                CryptographyService, 
                relayServerService, 
                sessionCryptographyServiceLogger);
            
            var matrixClient = MatrixClientFactory.Create(pollingServiceLogger);
            
            var jsonSerializerService = new JsonSerializerService();

            var channelOpeningMessageBuilder = new ChannelOpeningMessageBuilder(
                keyPairRepository, 
                CryptographyService,
                jsonSerializerService);
            
            var p2PCommunicationService = new P2PCommunicationService(
                sessionCryptographyService, 
                matrixClient, 
                channelOpeningMessageBuilder);

            _client = new WalletBeaconClient(
                p2PCommunicationService, 
                beaconPeerRepository, 
                jsonSerializerService, 
                options);

            return _client;
        }
    }
}