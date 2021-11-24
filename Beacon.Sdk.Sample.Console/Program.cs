namespace Beacon.Sdk.Sample.Console
{
    using System;
    using System.Threading.Tasks;
    using Core.Domain.P2P;
    using Core.Domain.P2P.Dto;
    using Core.Domain.Services;
    using Matrix.Sdk.Core.Domain.Services;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Serilog;
    using Serilog.Sinks.SystemConsole.Themes;

    internal class Program
    {
        private static IHostBuilder CreateHostBuilder() => new HostBuilder()
            .ConfigureServices((hostContext, services) =>
            {
                services.AddLogging(loggingBuilder => 
                    loggingBuilder.AddSerilog(dispose: true));
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
            ILogger<P2PLoginRequestFactory> relayServerServiceLogger = host.Services.GetRequiredService<ILogger<P2PLoginRequestFactory>>();
            ILogger<P2PMessageService> sessionCryptographyServiceLogger = host.Services.GetRequiredService<ILogger<P2PMessageService>>();
            ILogger<PollingService> pollingServiceLogger = host.Services.GetRequiredService<ILogger<PollingService>>();
           
            logger.LogInformation("START");
            var sample = new Sample(relayServerServiceLogger, sessionCryptographyServiceLogger, pollingServiceLogger);

            try
            {
                await sample.Run();
                // sample.TestRepositories();

            }
            catch (Exception ex)
            {
                throw;
            }
            
            
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