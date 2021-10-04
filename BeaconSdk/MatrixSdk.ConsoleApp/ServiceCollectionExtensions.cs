namespace MatrixSdk.ConsoleApp
{
    using Microsoft.Extensions.DependencyInjection;
    using Serilog;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddConsoleApp(this IServiceCollection services)
        {
            services.AddLogging(loggingBuilder =>
                loggingBuilder.AddSerilog(dispose: true));

            return services;
        }
    }
}