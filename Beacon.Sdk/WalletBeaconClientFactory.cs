namespace Beacon.Sdk
{
    using Core.Domain.P2P.ChannelOpening;
    using Core.Domain.Services;
    using Core.Domain.Services.P2P;
    using Core.Infrastructure;
    using Core.Infrastructure.Cryptography;
    using Core.Infrastructure.Repositories;
    using Matrix.Sdk;
    using Matrix.Sdk.Core.Domain.Services;
    using Matrix.Sdk.Core.Infrastructure.Services;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    public class WalletBeaconClientFactory
    {
        private static readonly MatrixClientFactory MatrixClientFactory = new();
        private static readonly SingletonHttpFactory SingletonHttpFactory = new();

        private IWalletBeaconClient? _client;

        public IWalletBeaconClient Create(
            WalletBeaconClientOptions options,
            ILogger<RelayServerService>? relayServerServiceLogger,
            ILogger<PollingService> pollingServiceLogger)
        {
            if (_client != null)
                return _client;

            #region Infrastructure

            var cryptographyService = new CryptographyService();
            var sessionKeyPairRepository = new InMemorySessionKeyPairRepository(cryptographyService);

            var beaconPeerRepository = new LiteDbBeaconPeerRepository(new NullLogger<LiteDbBeaconPeerRepository>());
            var peerRoomRepository = new LiteDbPeerRoomRepository(new NullLogger<LiteDbPeerRoomRepository>());
            var seedRepository = new LiteDbSeedRepository(new NullLogger<LiteDbSeedRepository>());
            var sdkStorage = new SdkStorage();
            var jsonSerializerService = new JsonSerializerService();

            #endregion

            #region Domain

            var keyPairService = new KeyPairService(cryptographyService, seedRepository);

            #endregion

            #region P2P

            var channelOpeningMessageBuilder =
                new ChannelOpeningMessageBuilder(cryptographyService, jsonSerializerService, keyPairService);

            var messageService = new MessageService(
                new NullLogger<MessageService>(),
                cryptographyService,
                beaconPeerRepository,
                sessionKeyPairRepository,
                keyPairService);

            var matrixClientService = new ClientService(SingletonHttpFactory);
            var relayServerService = new RelayServerService(
                relayServerServiceLogger,
                matrixClientService,
                sdkStorage,
                cryptographyService,
                keyPairService);

            var p2PCommunicationService = new P2PCommunicationService(
                messageService,
                MatrixClientFactory.Create(pollingServiceLogger),
                channelOpeningMessageBuilder,
                relayServerService);

            #endregion
            
            _client = new WalletBeaconClient(
                p2PCommunicationService,
                beaconPeerRepository,
                jsonSerializerService,
                keyPairService,
                cryptographyService,
                options);

            return _client;
        }
    }
}