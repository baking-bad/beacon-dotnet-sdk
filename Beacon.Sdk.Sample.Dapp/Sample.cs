namespace Beacon.Sdk.Sample.Dapp;

using System.Runtime.InteropServices;
using BeaconClients;
using BeaconClients.Abstract;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Extensions.Logging;
using ILogger = Serilog.ILogger;

public class Sample
{
    public async Task Run()
    {
        const string path = "dapp-sample.db";

        var options = new BeaconOptions
        {
            AppName = "Dapp sample",
            AppUrl = string.Empty,
            IconUrl = string.Empty,
            KnownRelayServers = Constants.KnownRelayServers,

            DatabaseConnectionString = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? $"Filename={path}; Connection=Shared;"
                : $"Filename={path}; Mode=Exclusive;"
        };

        ILogger serilogLogger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console()
            .CreateLogger();

        ILoggerProvider loggerProvider = new SerilogLoggerProvider(serilogLogger);

        IDappBeaconClient beaconDappClient = BeaconClientFactory.Create<IDappBeaconClient>(options, loggerProvider);
        // _beaconWalletClient.OnBeaconMessageReceived += OnBeaconWalletClientMessageReceived;
        // _beaconWalletClient.OnDappsListChanged += OnDappsListChanged;

        await beaconDappClient.InitAsync();
        beaconDappClient.Connect();
    }
}