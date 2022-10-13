namespace Beacon.Sdk.Core.Domain.Entities.P2P
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Infrastructure;
    using Interfaces;
    using Matrix.Sdk.Core.Infrastructure.Services;
    using Microsoft.Extensions.Logging;
    using Services;
    using Sodium;

    public class P2PLoginRequestFactory
    {
        private readonly ICryptographyService _cryptographyService;
        private readonly KeyPairService _keyPairService;
        private readonly ILogger<P2PLoginRequestFactory>? _logger;
        private readonly ClientService _matrixClientService;
        private readonly ISdkStorage _sdkStorage;

        public P2PLoginRequestFactory(
            ILogger<P2PLoginRequestFactory>? logger,
            ClientService matrixClientService,
            ISdkStorage sdkStorage,
            ICryptographyService cryptographyService,
            KeyPairService keyPairService)
        {
            _logger = logger;
            _matrixClientService = matrixClientService;
            _sdkStorage = sdkStorage;
            _cryptographyService = cryptographyService;
            _keyPairService = keyPairService;
        }

        public async Task<P2PLoginRequest> Create(string[] knownRelayServers)
        {
            KeyPair keyPair = _keyPairService.KeyPair;

            string relayServer = await GetRelayServer(knownRelayServers);

            byte[] loginDigest = _cryptographyService.GenerateLoginDigest();
            string hexSignature = _cryptographyService.GenerateHexSignature(loginDigest, keyPair.PrivateKey);
            string publicKeyHex = GetPublicKeyHex();
            string hexId = _cryptographyService.GenerateHexId(_keyPairService.KeyPair.PublicKey);

            var password = $"ed:{hexSignature}:{publicKeyHex}";
            string deviceId = publicKeyHex;

            var address = new Uri($@"https://{relayServer}");

            return new P2PLoginRequest(address, hexId, password, deviceId);
        }

        public string GetPublicKeyHex() => _cryptographyService.ToHexString(_keyPairService.KeyPair.PublicKey);
        public string GetPrivateKeyHex() => _cryptographyService.ToHexString(_keyPairService.KeyPair.PrivateKey);

        private static int PublicKeyToInt(byte[] input, int modulus)
        {
            int sum = input.Select((t, i) => t + i).Sum();

            return (int) Math.Floor((decimal) (sum % modulus));
        }

        public async Task<string> GetRelayServer(string[] knownRelayServers)
        {
            if (_sdkStorage.MatrixSelectedNode is {Length: > 0})
                return _sdkStorage.MatrixSelectedNode;

            int startIndex = PublicKeyToInt(_keyPairService.KeyPair.PublicKey, knownRelayServers.Length);
            var offset = 0;

            while (offset < knownRelayServers.Length)
            {
                int index = (startIndex + offset) % knownRelayServers.Length;
                string relayServer = knownRelayServers[index];

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
                    _logger?.LogInformation(ex, "Ignoring server \"{RelayServer}\", trying another one", relayServer);
                    offset++;
                }
            }

            throw new InvalidOperationException("No matrix server reachable!");
        }
    }
}