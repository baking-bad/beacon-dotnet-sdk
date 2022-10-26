namespace Beacon.Sdk.BeaconClients
{
    using System.IO;
    using System.Runtime.InteropServices;
    using Abstract;
    using Core.Domain;
    using Core.Domain.Entities;
    using Core.Domain.Entities.P2P;
    using Core.Domain.Interfaces;
    using Core.Domain.Interfaces.Data;
    using Core.Domain.P2P.ChannelOpening;
    using Core.Domain.Services;
    using Core.Infrastructure;
    using Core.Infrastructure.Cryptography;
    using Core.Infrastructure.Repositories;
    using Matrix.Sdk.Core.Infrastructure.Services;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using MatrixClientServiceExtensions = Matrix.Sdk.MatrixClientServiceExtensions;

    public static class BeaconClientServiceExtensions
    {
        public static IServiceCollection AddBeaconWalletClient(this IServiceCollection services,
            BeaconOptions? options = null,
            ILoggerProvider? loggerProvider = null)
        {
            services.AddBeaconClient(options, loggerProvider);
            services.AddSingleton<IWalletBeaconClient, WalletBeaconClient>();
            return services;
        }
        
        public static IServiceCollection AddBeaconDappClient(this IServiceCollection services,
            BeaconOptions? options = null,
            ILoggerProvider? loggerProvider = null)
        {
            services.AddBeaconClient(options, loggerProvider);
            services.AddSingleton<IDappBeaconClient, DappBeaconClient>();
            return services;
        }

        private static IServiceCollection AddBeaconClient(this IServiceCollection services,
            BeaconOptions? options = null,
            ILoggerProvider? loggerProvider = null)
        {
            var defaultBeaconOptions = new BeaconOptions
            {
                AppName = "Unknown beacon-dotnet-sdk client",
                AppUrl = string.Empty,
                IconUrl = string.Empty,
                KnownRelayServers = Constants.KnownRelayServers,

                DatabaseConnectionString = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                    ? $"Filename={Directory.GetCurrentDirectory()}/beacon.db; Connection=Shared;"
                    : $"Filename={Directory.GetCurrentDirectory()}/beacon.db; Mode=Exclusive;"
            };

            BeaconOptions beaconOptions = options ?? defaultBeaconOptions;

            if (loggerProvider != null)
                services.AddLogging(builder => builder.AddProvider(loggerProvider));
            
            MatrixClientServiceExtensions.AddMatrixClient(services);
            services.AddSingleton<ICryptographyService, CryptographyService>();
            services.AddSingleton<BeaconOptions>(beaconOptions);

            #region Infrastructure

            services.AddSingleton(new RepositorySettings
            {
                ConnectionString = beaconOptions.DatabaseConnectionString
            });

            services.AddSingleton<ISessionKeyPairRepository, InMemorySessionKeyPairRepository>();
            services.AddSingleton<IPeerRepository, LiteDbPeerRepository>();
            services.AddSingleton<IP2PPeerRoomRepository, LiteDbP2PPeerRoomRepository>();
            services.AddSingleton<ISeedRepository, LiteDbSeedRepository>();
            services.AddSingleton<IAppMetadataRepository, LiteDbAppMetadataRepository>();
            services.AddSingleton<IPermissionInfoRepository, LiteDbPermissionInfoRepository>();
            services.AddSingleton<IMatrixSyncRepository, LiteDbMatrixSyncRepository>();
            services.AddSingleton<ISdkStorage, SdkStorage>();
            services.AddSingleton<IJsonSerializerService, JsonSerializerService>();

            #endregion


            #region Domain

            services.AddSingleton<KeyPairService>();
            services.AddSingleton<AccountService>();

            services.AddSingleton<PeerFactory>();
            services.AddSingleton<PermissionInfoFactory>();
            
            services.AddSingleton<DeserializeMessageHandler>();
            services.AddSingleton<SerializeMessageHandler>();

            #endregion

            #region P2P

            services.AddSingleton<IChannelOpeningMessageBuilder, ChannelOpeningMessageBuilder>();
            services.AddSingleton<P2PMessageService>();
            services.AddSingleton<ClientService>();
            services.AddSingleton<P2PLoginRequestFactory>();
            services.AddSingleton<P2PPeerRoomFactory>();
            services.AddSingleton<IP2PCommunicationService, P2PCommunicationService>();

            #endregion
            
            return services;
        }
    }
}