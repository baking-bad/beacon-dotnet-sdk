namespace Beacon.Sdk
{
    using System;
    using System.Threading.Tasks;
    using Core;
    using Core.Transport.P2P.Dto.Handshake;
    using Matrix.Sdk.Core.Utils;
    using Sodium;

    public class WalletBeaconClient
    {
        private readonly ICryptographyService _cryptographyService;
        private readonly IP2PClient _p2PClient;
        public readonly string AppName;

        private KeyPair? _keyPair;

        public WalletBeaconClient(ICryptographyService cryptographyService, IP2PClient p2PClient,
            WalletBeaconClientOptions options)
        {
            _cryptographyService = cryptographyService;
            _p2PClient = p2PClient;

            AppName = options.AppName;
        }

        private KeyPair ReadBeaconSecret()
        {
            // Todo: add storage
            if (_keyPair != null)
                return _keyPair;

            var seed = Guid.NewGuid().ToString();
            _keyPair = _cryptographyService.GenerateEd25519KeyPair(seed);

            return _keyPair;
        }

        public async Task StartAsync()
        {
            _keyPair = ReadBeaconSecret();
            await _p2PClient.StartAsync(_keyPair);
        }

        public async Task AddPeerAsync(P2PPairingRequest pairingRequest)
        {
            if (!HexString.TryParse(pairingRequest.PublicKey, out HexString receiverPublicKeyHex))
                throw new InvalidOperationException("Can not parse receiver public key.");

            string pairingRequestId =
                pairingRequest.Id ?? throw new NullReferenceException("Provide pairing request id.");
            var version = int.Parse(pairingRequest.Version);

            var pairingResponse = new PairingResponse(
                pairingRequestId,
                receiverPublicKeyHex,
                pairingRequest.RelayServer,
                version,
                AppName,
                "beacon-node-0.papers.tech:8448");

            await _p2PClient.SendPairingResponseAsync(pairingResponse);
        }

        public void Connect()
        {
        }
    }
}