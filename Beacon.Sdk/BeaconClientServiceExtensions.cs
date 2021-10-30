namespace Beacon.Sdk
{
    using Core.Infrastructure.Repositories;
    using Core.Infrastructure.Serialization;
    using Core.Transport.P2P;
    using Core.Transport.P2P.ChannelOpening;
    using Matrix.Sdk;
    using Microsoft.Extensions.DependencyInjection;

    public static class BeaconClientServiceExtensions
    {
        public static IServiceCollection AddBeaconClient(this IServiceCollection services)
        {
            services.AddMatrixClient();
            services.AddSingleton<ISdkStorage, SdkStorage>();
            services.AddSingleton<RelayServerService>();
            services.AddSingleton<JsonSerializerService>();
            services.AddSingleton<IChannelOpeningMessageBuilder, ChannelOpeningMessageBuilder>();

            services.AddHttpClient("test", configureClient => { });
            return services;
        }
    }
}