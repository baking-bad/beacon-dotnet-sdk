namespace Beacon.Sdk.Core.Domain.Services.P2P
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Infrastructure.Repositories;
    using Interfaces;
    using Matrix.Sdk.Core.Infrastructure.Services;
    using Microsoft.Extensions.Logging;
    using Sodium;

    public class RelayServerService
    {
        private readonly ICryptographyService _cryptographyService;
        private readonly KeyPairService _keyPairService;
        private readonly ILogger<RelayServerService>? _logger;
        private readonly ClientService _matrixClientService;
        private readonly ISdkStorage _sdkStorage;

        public RelayServerService(ILogger<RelayServerService>? logger, ClientService matrixClientService,
            ISdkStorage sdkStorage, ICryptographyService cryptographyService, KeyPairService keyPairService)
        {
            _logger = logger;
            _matrixClientService = matrixClientService;
            _sdkStorage = sdkStorage;
            _cryptographyService = cryptographyService;
            _keyPairService = keyPairService;
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

        public async Task<LoginRequest> CreateLoginRequest()
        {
            KeyPair keyPair = _keyPairService.KeyPair;
            string relayServer = await GetRelayServer(keyPair.PublicKey);

            byte[] loginDigest = _cryptographyService.GenerateLoginDigest();
            string hexSignature = _cryptographyService.GenerateHexSignature(loginDigest, keyPair.PrivateKey);
            string publicKeyHex = _cryptographyService.ToHexString(keyPair.PublicKey);
            string hexId = _cryptographyService.GenerateHexId(keyPair.PublicKey);

            var password = $"ed:{hexSignature}:{publicKeyHex}";
            string deviceId = publicKeyHex;

            var address = new Uri($@"https://{relayServer}");

            return new LoginRequest(address, hexId, password, deviceId);
        }

        public record LoginRequest(Uri Address, string Username, string Password, string DeviceId);
    }
}