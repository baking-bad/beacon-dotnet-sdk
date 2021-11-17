namespace Beacon.Examples.ConsoleApp
{
    using System;
    using System.Threading.Tasks;
    using Matrix.Examples.ConsoleApp;
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

// var numbers = new []{1, 2, 3, 4, 5, 6, 7, 8, 9, 10};
// foreach (var number in numbers)
//     Console.Write($"{number} ");
// Console.WriteLine();
//
// foreach (var i in numbers[1..5])
//     Console.Write($"{i} ");
// Console.WriteLine();

// EncryptionService.DecryptWithSharedKey(null, null);

// var a1 = "0xFAFA";
// if (HexString.TryParse(a1, out var r1)) 
//     Console.WriteLine(r1);
//
// var a2 = "0xFAFA";
// if (HexString.TryParse(a2, out var r2)) 
//     Console.WriteLine(r2);
//
// Console.WriteLine(r1 == r2);
//
//
// var a3 = "0x38";
// HexString.TryParse(a3, out var r3);
// Console.WriteLine(r3.ToASCII());
//
// var b3 = r3.ToByteArray();
// Console.WriteLine(r3.ToASCII());


// var a3 = "48656c6c6f20576f726c6421";
// if (HexString.TryParse(a3, out var r3)) 
//     Console.WriteLine(r3.ToASCII());

// var t = new HexString();
// Console.WriteLine(t.ToASCII());нну

// const string a3 = "0xFAF";
// _ = HexString.TryParse(a3, out var r3);
// Assert.AreEqual(r3.Value,string.Empty);