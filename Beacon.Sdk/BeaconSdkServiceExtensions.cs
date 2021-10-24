namespace Beacon.Sdk
{
    using Core.Infrastructure.Repositories;
    using Core.Infrastructure.Serialization;
    using Core.Infrastructure.Transport.P2P;
    using Core.Infrastructure.Transport.P2P.ChannelOpening;
    using Matrix.Sdk;
    using Microsoft.Extensions.DependencyInjection;

    public static class BeaconSdkServiceExtensions
    {
        public static IServiceCollection AddBeaconSdk(this IServiceCollection services)
        {
            services.AddMatrixSdk();
            services.AddSingleton<ISdkStorage, SdkStorage>();
            services.AddSingleton<RelayServerService>();
            services.AddSingleton<JsonSerializerService>();
            services.AddSingleton<IChannelOpeningMessageBuilder, ChannelOpeningMessageBuilder>();

            return services;
        }
    }
}