namespace Beacon.Sdk.Core.Domain.P2P
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Dto;
    using Interfaces;
    using Services;
    using Infrastructure.Repositories;
    using Matrix.Sdk.Core.Infrastructure.Services;
    using Microsoft.Extensions.Logging;
    using Sodium;

    public class P2PLoginRequestFactory
    {
        private readonly ILogger<P2PLoginRequestFactory>? _logger;
        private readonly ClientService _matrixClientService;
        private readonly ISdkStorage _sdkStorage;
        private readonly ICryptographyService _cryptographyService;
        private readonly KeyPairService _keyPairService;

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
        
        public async Task<P2PLoginRequest> Create()
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

            return new P2PLoginRequest(address, hexId, password, deviceId);
        }

        private static int PublicKeyToInt(byte[] input, int modulus)
        {
            int sum = input.Select((t, i) => t + i).Sum();

            return (int) Math.Floor((decimal) (sum % modulus));
        }

        private async Task<string> GetRelayServer(byte[] publicKey)
        {
            if (_sdkStorage.MatrixSelectedNode is {Length: > 0})
                return _sdkStorage.MatrixSelectedNode;

            int startIndex = PublicKeyToInt(publicKey, BeaconConstants.KnownRelayServers.Length);
            var offset = 0;

            while (offset < BeaconConstants.KnownRelayServers.Length)
            {
                int index = (startIndex + offset) % BeaconConstants.KnownRelayServers.Length;
                string relayServer = BeaconConstants.KnownRelayServers[index];

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