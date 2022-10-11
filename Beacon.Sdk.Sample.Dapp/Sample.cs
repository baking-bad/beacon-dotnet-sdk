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
            AppUrl = "https://awesome-dapp.com",
            IconUrl = "https://bcd-static-assets.fra1.digitaloceanspaces.com/dapps/atomex/atomex_logo.jpg",
            KnownRelayServers = Constants.KnownRelayServers,

            DatabaseConnectionString = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? $"Filename={path}; Connection=Shared;"
                : $"Filename={path}; Mode=Exclusive;"
        };

        ILogger logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console()
            .CreateLogger();

        ILoggerProvider loggerProvider = new SerilogLoggerProvider(logger);
        IDappBeaconClient beaconDappClient = BeaconClientFactory.Create<IDappBeaconClient>(options, loggerProvider);
        beaconDappClient.OnBeaconMessageReceived += OnBeaconWalletClientMessageReceived;
        beaconDappClient.OnDappsListChanged += OnDappsListChanged;

        await beaconDappClient.InitAsync();
        beaconDappClient.Connect();

        string pairingRequestQrData = await beaconDappClient.GetPairingRequestInfo();
        logger.Information("QR CODE Is\n{Qr}", pairingRequestQrData);
    }

    private void OnDappsListChanged(object? sender, DappConnectedEventArgs? e)
    {
        var a = 5;
    }

    private void OnBeaconWalletClientMessageReceived(object? sender, BeaconMessageEventArgs e)
    {
        var a = 5;
    }
}