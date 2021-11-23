namespace Beacon.Sdk.Sample.Console
{
    using System;
    using System.Threading.Tasks;
    using Core.Domain.Services;
    using Core.Domain.Services.P2P;
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
            ILogger<RelayServerService> relayServerServiceLogger = host.Services.GetRequiredService<ILogger<RelayServerService>>();
            ILogger<MessageService> sessionCryptographyServiceLogger = host.Services.GetRequiredService<ILogger<MessageService>>();
            ILogger<PollingService> pollingServiceLogger = host.Services.GetRequiredService<ILogger<PollingService>>();
           
            logger.LogInformation("START");
            var sample = new Sample(relayServerServiceLogger, sessionCryptographyServiceLogger, pollingServiceLogger);

            try
            {
                // await sample.Run();
                sample.Test();

            }
            catch (Exception ex)
            {
                
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