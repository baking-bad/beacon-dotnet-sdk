namespace Beacon.Sdk.Core.Transport.P2P
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Infrastructure.Repositories;
    using Matrix.Sdk.Core.Infrastructure.Services;
    using Microsoft.Extensions.Logging;

    public class RelayServerService
    {
        private readonly ILogger<RelayServerService>? _logger;
        private readonly ClientService _matrixClientService;
        private readonly ISdkStorage _sdkStorage;

        public RelayServerService(ILogger<RelayServerService>? logger, ClientService matrixClientService,
            ISdkStorage sdkStorage)
        {
            _logger = logger;
            _matrixClientService = matrixClientService;
            _sdkStorage = sdkStorage;
        }

        private static int PublicKeyToInt(byte[] input, int modulus)
        {
            int sum = input.Select((t, i) => t + i).Sum();

            return (int) Math.Floor((decimal) (sum % modulus));
        }

        public async Task<string> GetRelayServer(byte[] publicKey)
        {
            if (_sdkStorage.MatrixSelectedNode is {Length: > 0})
                return _sdkStorage.MatrixSelectedNode;

            int startIndex = PublicKeyToInt(publicKey, Constants.KnownRelayServers.Length);
            var offset = 0;

            while (offset < Constants.KnownRelayServers.Length)
            {
                int index = (startIndex + offset) % Constants.KnownRelayServers.Length;
                string relayServer = Constants.KnownRelayServers[index];

                try
                {
                    var cts = new CancellationTokenSource();
                    var address = new Uri($@"https://{relayServer}");
                    _matrixClientService.BaseAddress = address;
                    _ = await _matrixClientService.GetMatrixClientVersions(address, cts.Token);

                    _sdkStorage.MatrixSelectedNode = relayServer;

                    return _sdkStorage.MatrixSelectedNode;
                }
                catch (Exception ex)
                {
                    _logger?.LogInformation($"Ignoring server \"{relayServer}\", trying another one...");
                    offset++;
                }
            }

            throw new InvalidOperationException("No matrix server reachable!");
        }
    }
}