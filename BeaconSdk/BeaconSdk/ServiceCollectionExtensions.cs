namespace BeaconSdk
{
    using MatrixSdk;
    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBeaconSdk(this IServiceCollection services)
        {
            services.AddMatrixSdk();

            return services;
        }
    }
}