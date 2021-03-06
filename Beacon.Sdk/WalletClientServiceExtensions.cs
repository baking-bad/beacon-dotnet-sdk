namespace Beacon.Sdk
{
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
    using Matrix.Sdk;
    using Matrix.Sdk.Core.Infrastructure.Services;
    using Microsoft.Extensions.DependencyInjection;
    using WalletBeaconClient;

    public static class WalletClientServiceExtensions
    {
        public static IServiceCollection AddBeaconClient(this IServiceCollection services)
        {
            services.AddMatrixClient();
            
            services.AddSingleton<ICryptographyService, CryptographyService>();

            services.AddSingleton<BeaconOptions>(new BeaconOptions
            {
                AppName = "Atomex Mobile",
                AppUrl = "", //string?
                IconUrl = "", // string?
                KnownRelayServers = new[]
                {
                    "beacon-node-0.papers.tech:8448",
                    "beacon-node-1.diamond.papers.tech",
                    "beacon-node-1.sky.papers.tech",
                    "beacon-node-2.sky.papers.tech",
                    "beacon-node-1.hope.papers.tech",
                    "beacon-node-1.hope-2.papers.tech",
                    "beacon-node-1.hope-3.papers.tech",
                    "beacon-node-1.hope-4.papers.tech",
                },
                // see https://github.com/mbdavid/LiteDB/issues/787
                DatabaseConnectionString = $"Filename=test1.db; Mode=Exclusive", // mac m1
                // DatabaseConnectionString = $"Filename={path}"
            });
            #region Infrastructure

            services.AddSingleton<RepositorySettings>(new RepositorySettings
            {
                ConnectionString = $"Filename=test1.db; Mode=Exclusive"
            });

            services.AddSingleton<ISessionKeyPairRepository, InMemorySessionKeyPairRepository>();
            
            services.AddSingleton<IPeerRepository, LiteDbPeerRepository>();
            services.AddSingleton<IP2PPeerRoomRepository, LiteDbP2PPeerRoomRepository>();
            services.AddSingleton<ISeedRepository, LiteDbSeedRepository>();
            services.AddSingleton<IAppMetadataRepository, LiteDbAppMetadataRepository>();
            services.AddSingleton<IPermissionInfoRepository, LiteDbPermissionInfoRepository>();
            services.AddSingleton<ISdkStorage, SdkStorage>();
            services.AddSingleton<IJsonSerializerService, JsonSerializerService>();

            #endregion

            
            #region Domain
            
            services.AddSingleton<KeyPairService>();
            services.AddSingleton<AccountService>();
           
            services.AddSingleton<PeerFactory>();
            services.AddSingleton<PermissionInfoFactory>();
            
            services.AddSingleton<PermissionHandler>();
            services.AddSingleton<RequestMessageHandler>();
            services.AddSingleton<ResponseMessageHandler>();
            
            #endregion
            
            #region P2P
            
            services.AddSingleton<IChannelOpeningMessageBuilder, ChannelOpeningMessageBuilder>();
            services.AddSingleton<P2PMessageService>();
            services.AddSingleton<ClientService>();
            services.AddSingleton<P2PLoginRequestFactory>();
            services.AddSingleton<P2PPeerRoomFactory>();
            services.AddSingleton<IP2PCommunicationService, P2PCommunicationService>();
            
            #endregion



            // services.AddSingleton<IPeerRepository, LiteDbPeerRepository>();
            // services.AddSingleton<ISdkStorage, SdkStorage>();
            // services.AddSingleton<RelayServerService>();
            // services.AddSingleton<JsonSerializerService>();
            // services.AddSingleton<IChannelOpeningMessageBuilder, ChannelOpeningMessageBuilder>();
            //
            // services.AddHttpClient("test", configureClient => { });
            
            services.AddSingleton<IWalletBeaconClient, WalletBeaconClient.WalletBeaconClient>();
            
            return services;
        }
    }
}