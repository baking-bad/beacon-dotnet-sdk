namespace Beacon.Sdk.Sample.Console
{
    using System.Threading.Tasks;
    using Matrix.Sdk;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Serilog;
    using Serilog.Sinks.SystemConsole.Themes;
    using ILogger = Microsoft.Extensions.Logging.ILogger;

    internal class Program
    {
        private static readonly DependencyInjectionSample DependencyInjectionSample = new();
        private static readonly Sample Sample = new();

#pragma warning disable CA1416
        private static IHostBuilder CreateHostBuilder() => new HostBuilder()
            .ConfigureServices((hostContext, services) =>
            {
                services.AddLogging(loggingBuilder => 
                    loggingBuilder.AddSerilog(dispose: true));

                services.AddMatrixClient();
                services.AddBeaconClient();
                
            }).UseConsoleLifetime();
        
#pragma warning restore CA1416

        private static async Task<int> Main(string[] args)
        {
            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddFilter("Microsoft", LogLevel.Warning)
                    .AddFilter("System", LogLevel.Warning)
                    .AddFilter("LoggingConsoleApp.Program", LogLevel.Debug)
                    .AddConsole();
            });
            
            ILogger logger = loggerFactory.CreateLogger<Program>();
            logger.LogInformation("Example log message");
            
            await Sample.Run();
            
            // IHost host = CreateHostBuilder().Build();
            // ILogger<Program> logger = host.Services.GetRequiredService<ILogger<Program>>();
            //
            // SystemConsoleTheme theme = LoggerSetup.SetupTheme();
            // Log.Logger = new LoggerConfiguration()
            //     .Enrich.FromLogContext()
            //     .WriteTo.Console(theme: theme)
            //     .CreateLogger();
            //
            // logger.LogInformation("START");
            //
            // await Sample.Run();
            // // await DependencyInjectionSample.Run(host.Services);
            //
            // logger.LogInformation("STOP");
            
            return 0;
        }
    }
}