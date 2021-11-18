namespace Beacon.Examples.ConsoleApp
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Sdk.Sample.Console;

    internal class Program
    {
        private static IHostBuilder CreateHostBuilder() => new HostBuilder()
            .ConfigureServices((hostContext, services) =>
            {
                // services.AddBeaconClient();
                // services.AddConsoleApp();
            }).UseConsoleLifetime();

        private static async Task<int> Main(string[] args)
        {
            IHost host = CreateHostBuilder().Build();

            SystemConsoleTheme theme = LoggerSetup.SetupTheme();
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console(theme: theme)
                .CreateLogger();

            ILogger<Program> logger = host.Services.GetRequiredService<ILogger<Program>>();

            logger.LogInformation("START");

            await RunAsync(host.Services);

            return 0;
        }

        private static async Task RunAsync(IServiceProvider serviceProvider)
        {
            await BeaconClientScenarios.Setup(serviceProvider);
        }
    }
}