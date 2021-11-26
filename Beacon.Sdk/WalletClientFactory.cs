namespace Beacon.Sdk
{
    using Core.Domain;
    using Core.Domain.P2P;
    using Core.Domain.P2P.ChannelOpening;
    using Core.Domain.Services;
    using Core.Infrastructure;
    using Core.Infrastructure.Cryptography;
    using Core.Infrastructure.Repositories;
    using Matrix.Sdk;
    using Matrix.Sdk.Core.Domain.Services;
    using Matrix.Sdk.Core.Infrastructure.Services;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;
    using WalletClient;

    public class WalletClientFactory
    {
        private static readonly MatrixClientFactory MatrixClientFactory = new();
        private static readonly SingletonHttpFactory SingletonHttpFactory = new();

        private IWalletClient? _client;

        public IWalletClient Create(
            BeaconOptions options,
            ILoggerFactory? loggerFactory = null)

        {
            if (_client != null)
                return _client;

            #region Infrastructure

            var repositorySettings = new RepositorySettings
            {
                ConnectionString = "test1.db"
            };
            var cryptographyService = new CryptographyService();
            var sessionKeyPairRepository = new InMemorySessionKeyPairRepository(cryptographyService);

            var peerRepository =
                new LiteDbPeerRepository(
                    new Logger<LiteDbPeerRepository>(
                        loggerFactory ?? NullLoggerFactory.Instance), repositorySettings);

            var p2PPeerRoomRepository =
                new LiteDbP2PPeerRoomRepository(
                    new Logger<LiteDbP2PPeerRoomRepository>(
                        loggerFactory ?? NullLoggerFactory.Instance), repositorySettings);

            var seedRepository =
                new LiteDbSeedRepository(
                    new Logger<LiteDbSeedRepository>(
                        loggerFactory ?? NullLoggerFactory.Instance), repositorySettings);

            var appMetadataRepository = new LiteDbAppMetadataRepository(
                new Logger<LiteDbAppMetadataRepository>(loggerFactory ?? NullLoggerFactory.Instance),
                repositorySettings);

            var sdkStorage = new SdkStorage();
            var jsonSerializerService = new JsonSerializerService();

            #endregion

            #region Domain

            var keyPairService = new KeyPairService(cryptographyService, seedRepository);

            #endregion

            #region P2P

            var channelOpeningMessageBuilder =
                new ChannelOpeningMessageBuilder(cryptographyService, jsonSerializerService, keyPairService);

            var p2PMessageService = new P2PMessageService(
                cryptographyService,
                sessionKeyPairRepository,
                keyPairService);

            var matrixClientService = new ClientService(SingletonHttpFactory);

            var p2PLoginRequestFactory = new P2PLoginRequestFactory(
                new Logger<P2PLoginRequestFactory>(loggerFactory ?? NullLoggerFactory.Instance),
                matrixClientService,
                sdkStorage,
                cryptographyService,
                keyPairService);

            var p2PPeerRoomFactory = new P2PPeerRoomFactory(cryptographyService);
            var p2PCommunicationService = new P2PCommunicationService(
                new Logger<P2PCommunicationService>(loggerFactory ?? NullLoggerFactory.Instance),
                MatrixClientFactory.Create(new Logger<PollingService>(loggerFactory ?? NullLoggerFactory.Instance)),
                channelOpeningMessageBuilder,
                p2PPeerRoomRepository,
                cryptographyService,
                p2PLoginRequestFactory,
                p2PPeerRoomFactory,
                p2PMessageService);

            #endregion

            var incomingMessageHandler = new IncomingMessageHandler(appMetadataRepository, jsonSerializerService);

            var peerFactory = new PeerFactory(cryptographyService);
            _client = new WalletBeaconClient(
                new Logger<WalletBeaconClient>(loggerFactory ?? new NullLoggerFactory()),
                peerRepository,
                appMetadataRepository,
                p2PCommunicationService,
                jsonSerializerService,
                keyPairService,
                peerFactory,
                incomingMessageHandler,
                options);

            return _client;
        }
    }
}