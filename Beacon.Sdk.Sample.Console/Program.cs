namespace Beacon.Sdk.Sample.Console
{
    using System;
    using System.Threading.Tasks;
    using Core.Domain.P2P;
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

            logger.LogInformation("START");
            
            
            var sample = new Sample();
            try
            {
                await sample.Run();

            }
            catch (Exception ex)
            {
                
            }
            // sample.TestRepositories();


            logger.LogInformation("STOP");

            await RunAsync(host.Services);

            return 0;
        }

        private static async Task RunAsync(IServiceProvider serviceProvider)
        {
            await BeaconClientScenarios.Setup(serviceProvider);
        }
    }
}