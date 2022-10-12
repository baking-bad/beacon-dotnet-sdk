// ReSharper disable ArgumentsStyleNamedExpression
// ReSharper disable ArgumentsStyleOther
// ReSharper disable ArgumentsStyleStringLiteral

using System.Runtime.InteropServices;
using Serilog;

namespace Beacon.Sdk.Sample.Console
{
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Beacon;
    using BeaconClients;
    using BeaconClients.Abstract;
    using Microsoft.Extensions.Logging;
    using Netezos.Encoding;
    using Newtonsoft.Json;
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

            System.Console.WriteLine("Enter qrcode:");
            var qrCodeString = System.Console.ReadLine();
            var decodedQr = Base58.Parse(qrCodeString);
            var message = Encoding.UTF8.GetString(decodedQr.ToArray());
            P2PPairingRequest pairingRequest = JsonConvert.DeserializeObject<P2PPairingRequest>(message);
            
            await beaconWalletClient.AddPeerAsync(pairingRequest, "");
        }
    }
}