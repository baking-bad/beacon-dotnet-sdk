namespace Beacon.Sdk
{
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
            WalletClientOptions options,
            ILoggerFactory? loggerFactory = null)

        {
            if (_client != null)
                return _client;

            #region Infrastructure

            var repositorySettings = new RepositorySettings
            {
                ConnectionString = ""
            };
            var cryptographyService = new CryptographyService();
            var sessionKeyPairRepository = new InMemorySessionKeyPairRepository(cryptographyService);

            var peerRepository =
                new LiteDbPeerRepository(
                    new Logger<LiteDbPeerRepository>(
                        loggerFactory ?? new NullLoggerFactory()), repositorySettings);

            var peerRoomRepository =
                new LiteDbPeerRoomRepository(
                    new Logger<LiteDbPeerRoomRepository>(
                        loggerFactory ?? new NullLoggerFactory()), repositorySettings);

            var seedRepository =
                new LiteDbSeedRepository(
                    new Logger<LiteDbSeedRepository>(
                        loggerFactory ?? new NullLoggerFactory()), repositorySettings);

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
                new Logger<P2PMessageService>(loggerFactory),
                cryptographyService,
                peerRepository,
                sessionKeyPairRepository,
                keyPairService);

            var matrixClientService = new ClientService(SingletonHttpFactory);
            var relayServerService = new P2PLoginRequestFactory(
                new Logger<P2PLoginRequestFactory>(loggerFactory ?? new NullLoggerFactory()),
                matrixClientService,
                sdkStorage,
                cryptographyService,
                keyPairService);

            var p2PCommunicationService = new P2PCommunicationService(
                p2PMessageService,
                MatrixClientFactory.Create(new Logger<PollingService>(loggerFactory ?? new NullLoggerFactory())),
                channelOpeningMessageBuilder,
                relayServerService);

            #endregion

            _client = new WalletClient.WalletClient(
                new Logger<WalletClient.WalletClient>(loggerFactory ?? new NullLoggerFactory()),
                peerRepository,
                peerRoomRepository,
                cryptographyService,
                p2PCommunicationService,
                jsonSerializerService,
                keyPairService,
                options);

            return _client;
        }
    }
}