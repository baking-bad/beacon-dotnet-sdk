// ReSharper disable ArgumentsStyleNamedExpression
// ReSharper disable ArgumentsStyleOther
// ReSharper disable ArgumentsStyleStringLiteral

using System.Runtime.InteropServices;
using Serilog;

namespace Beacon.Sdk.Sample.Console
{ 
    using System.Threading.Tasks;
    using BeaconClients;
    using BeaconClients.Abstract;
    using Microsoft.Extensions.Logging;
    using Serilog.Extensions.Logging;
    using ILogger = Serilog.ILogger;

    public class Sample
    {
        public async Task Run()
        {
            const string path = "wallet-beacon-sample.db";

            var options = new BeaconOptions
            {
                AppName = "Wallet sample",
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
            
            IWalletBeaconClient beaconWalletClient = BeaconClientFactory.Create<IWalletBeaconClient>(options, loggerProvider);
            // _beaconWalletClient.OnBeaconMessageReceived += OnBeaconWalletClientMessageReceived;
            // _beaconWalletClient.OnDappsListChanged += OnDappsListChanged;
            
            await beaconWalletClient.InitAsync();
            beaconWalletClient.Connect();
        }
    }
}