namespace BeaconSdk
{
    using Infrastructure.Repositories;
    using Infrastructure.Serialization;
    using Infrastructure.Transport.P2P;
    using Infrastructure.Transport.P2P.ChannelOpening;
    using MatrixSdk;
    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceCollectionExtensions
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