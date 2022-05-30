namespace Beacon.Sdk.Sample.Console
{
    using System.Threading.Tasks;
    using Matrix.Sdk;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Serilog;

    internal class Program
    {
        private static readonly DependencyInjectionSample DependencyInjectionSample = new();
        private static readonly Sample Sample = new();
        
        private static IHostBuilder CreateHostBuilder() => new HostBuilder()
            .ConfigureServices((hostContext, services) =>
            {
                services.AddLogging(loggingBuilder =>
                    loggingBuilder.AddSerilog(dispose: true));

                services.AddMatrixClient();
                services.AddBeaconClient();
            }).UseConsoleLifetime();


        private static async Task<int> Main(string[] args)
        {
            // using var loggerFactory = LoggerFactory.Create(builder =>
            // {
            //     builder
            //         .AddFilter("Microsoft", LogLevel.Warning)
            //         .AddFilter("System", LogLevel.Warning)
            //         .AddFilter("LoggingConsoleApp.Program", LogLevel.Debug)
            //         .AddConsole();
            // });
            //
            // ILogger logger = loggerFactory.CreateLogger<Program>();
            // logger.LogInformation("Example log message");

            // SystemConsoleTheme theme = LoggerSetup.SetupTheme();
            // Log.Logger = new LoggerConfiguration()
            //     .Enrich.FromLogContext()
            //     .WriteTo.Console(theme: theme)
            //     .CreateLogger();
            //
            // Log.Information("LOGGING WORKED");
            //
            // await Sample.Run();

            var theme = LoggerSetup.SetupTheme();
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console(theme: theme)
                .CreateLogger();

            var host = CreateHostBuilder().Build();

            Log.Information("START");
            await DependencyInjectionSample.Run(host.Services);
            Log.Information("STOP");

            return 0;
        }
    }
}