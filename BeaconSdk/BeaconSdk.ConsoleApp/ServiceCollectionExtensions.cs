namespace BeaconSdk.ConsoleApp
{
    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddConsoleApp(this IServiceCollection services) => services;
    }
}