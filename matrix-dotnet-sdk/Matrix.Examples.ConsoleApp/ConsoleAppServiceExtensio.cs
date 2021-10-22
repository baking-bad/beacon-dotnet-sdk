namespace Matrix.Examples.ConsoleApp
{
    using Microsoft.Extensions.DependencyInjection;
    using Serilog;

    public static class ConsoleAppServiceExtensio
    {
        public static IServiceCollection AddConsoleApp(this IServiceCollection services)
        {
            services.AddLogging(loggingBuilder =>
                loggingBuilder.AddSerilog(dispose: true));

            return services;
        }
    }
}