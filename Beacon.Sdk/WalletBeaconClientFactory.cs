namespace Beacon.Sdk
{
    using Core.Domain;
    using Core.Domain.Entities;
    using Core.Domain.Entities.P2P;
    using Core.Domain.P2P.ChannelOpening;
    using Core.Domain.Services;
    using Core.Infrastructure;
    using Core.Infrastructure.Cryptography;
    using Core.Infrastructure.Repositories;
    using Matrix.Sdk;
    using Matrix.Sdk.Core.Domain.Services;
    using Matrix.Sdk.Core.Infrastructure.Services;
    using Microsoft.Extensions.Logging;
    using WalletBeaconClient;

    public class WalletBeaconClientFactory
    {
        private static readonly MatrixClientFactory MatrixClientFactory = new();
        private static readonly SingletonHttpFactory SingletonHttpFactory = new();

        private IWalletBeaconClient? _client;

        public IWalletBeaconClient Create(
            BeaconOptions options,
            ILoggerFactory? loggerFactory = null)

        {
            #region Infrastructure

            var repositorySettings = new RepositorySettings
            {
                ConnectionString = options.DatabaseConnectionString
            };
            var cryptographyService = new CryptographyService();
            var sessionKeyPairRepository = new InMemorySessionKeyPairRepository(cryptographyService);

            
            var peerRepository = new LiteDbPeerRepository(loggerFactory.CreateLogger<LiteDbPeerRepository>(), repositorySettings);

            var p2PPeerRoomRepository = new LiteDbP2PPeerRoomRepository(loggerFactory.CreateLogger<LiteDbP2PPeerRoomRepository>(), repositorySettings);

            var seedRepository = new LiteDbSeedRepository(loggerFactory.CreateLogger<LiteDbSeedRepository>(), repositorySettings);

            var appMetadataRepository = new LiteDbAppMetadataRepository(loggerFactory.CreateLogger<LiteDbAppMetadataRepository>(), repositorySettings);

            var permissionInfoRepository = new LiteDbPermissionInfoRepository(loggerFactory.CreateLogger<LiteDbPermissionInfoRepository>(), repositorySettings);

            var matrixSyncRepository = new LiteDbMatrixSyncRepository(loggerFactory.CreateLogger<LiteDbMatrixSyncRepository>(), repositorySettings);
            
            var sdkStorage = new SdkStorage();
            var jsonSerializerService = new JsonSerializerService();

            #endregion

            #region Domain

            var keyPairService = new KeyPairService(cryptographyService, seedRepository);
            var accountService = new AccountService(cryptographyService);

            var peerFactory = new PeerFactory(cryptographyService);
            var permissionInfoFactory = new PermissionInfoFactory(accountService);

            var permissionHandler = new PermissionHandler(permissionInfoRepository, accountService);
            var incomingMessageHandler = new RequestMessageHandler(appMetadataRepository, jsonSerializerService);
            var outgoingMessageHandler = new ResponseMessageHandler(appMetadataRepository, permissionInfoRepository,
                jsonSerializerService, permissionInfoFactory);

            #endregion

            #region P2P

            var channelOpeningMessageBuilder =
                new ChannelOpeningMessageBuilder(cryptographyService, jsonSerializerService, keyPairService);

            var p2PMessageService = new P2PMessageService(
                cryptographyService,
                sessionKeyPairRepository,
                keyPairService);

            var matrixClientService = new ClientService(SingletonHttpFactory);

            var p2PLoginRequestFactory = new P2PLoginRequestFactory(loggerFactory.CreateLogger<P2PLoginRequestFactory>(),
                matrixClientService,
                sdkStorage,
                cryptographyService,
                keyPairService);

            var p2PPeerRoomFactory = new P2PPeerRoomFactory(cryptographyService);
            var p2PCommunicationService = new P2PCommunicationService(
                loggerFactory.CreateLogger<P2PCommunicationService>(),
                MatrixClientFactory.Create(loggerFactory.CreateLogger<PollingService>()),
                channelOpeningMessageBuilder,
                p2PPeerRoomRepository,
                matrixSyncRepository,
                cryptographyService,
                p2PLoginRequestFactory,
                p2PPeerRoomFactory,
                p2PMessageService);

            #endregion

            _client = new WalletBeaconClient.WalletBeaconClient(loggerFactory.CreateLogger<WalletBeaconClient.WalletBeaconClient>(),
                peerRepository,
                appMetadataRepository,
                permissionInfoRepository,
                seedRepository,
                p2PCommunicationService,
                accountService,
                keyPairService,
                peerFactory,
                incomingMessageHandler,
                outgoingMessageHandler,
                permissionHandler,
                options);

            var k = loggerFactory.CreateLogger<WalletBeaconClientFactory>();
            k.LogInformation("Init factory");

            return _client;
        }
    }
}