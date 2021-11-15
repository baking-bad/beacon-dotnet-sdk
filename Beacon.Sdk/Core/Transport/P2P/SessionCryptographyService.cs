namespace Beacon.Sdk.Core.Transport.P2P
{
    using System;
    using System.Threading.Tasks;
    using Domain;
    using Domain.Interfaces.Data;
    using Infrastructure.Cryptography.Libsodium;
    using Matrix.Sdk.Core.Domain.Network;
    using Matrix.Sdk.Core.Domain.Room;
    using Microsoft.Extensions.Logging;
    using Sodium;

    public class SessionCryptographyService
    {
        private readonly IKeyPairRepository _keyPairRepository;
        private readonly IBeaconPeerRepository _beaconPeerRepository;
        private readonly ICryptographyService _cryptographyService;
        private readonly RelayServerService _relayServerService;
        private readonly ILogger<SessionCryptographyService> _logger;

        public SessionCryptographyService(
            IKeyPairRepository keyPairRepository,
            IBeaconPeerRepository beaconPeerRepository,
            ICryptographyService cryptographyService,
            RelayServerService relayServerService,
            ILogger<SessionCryptographyService> logger)
        {
            _keyPairRepository = keyPairRepository;
            _beaconPeerRepository = beaconPeerRepository;
            _cryptographyService = cryptographyService;
            _relayServerService = relayServerService;
            _logger = logger;
        }

        public string? TryDecryptMessageFromEvent(BaseRoomEvent matrixRoomEvent)
        {
            if (matrixRoomEvent is not TextMessageEvent textMessageEvent) return null;

            string senderUserId = textMessageEvent.SenderUserId;
            BeaconPeer? peer = _beaconPeerRepository.TryReadByUserId(senderUserId);

            if (peer == null)
            {
                _logger.LogInformation("Unknown sender");
                return null;
            }

            if (!senderUserId.StartsWith(peer.UserId)) return null;

            if (!_cryptographyService.Validate(textMessageEvent.Message))
            {
                _logger.LogInformation("Can not validate message");
                return null;
            }

            KeyPair keyPair =  _keyPairRepository.KeyPair;

            SessionKeyPair serverSessionKeyPair =
                _keyPairRepository.CreateOrReadServerSession(peer.HexPublicKey, keyPair);

            return _cryptographyService.DecryptAsString(textMessageEvent.Message,
                serverSessionKeyPair.Rx);
        }

        public async Task<LoginRequest> CreateLoginRequest()
        {
            KeyPair keyPair = _keyPairRepository.KeyPair;
            string relayServer = await _relayServerService.GetRelayServer(keyPair.PublicKey);
            
            byte[] loginDigest = _cryptographyService.GenerateLoginDigest();
            string hexSignature = _cryptographyService.GenerateHexSignature(loginDigest, keyPair.PrivateKey);
            string publicKeyHex = _cryptographyService.ToHexString(keyPair.PublicKey);
            string hexId = _cryptographyService.GenerateHexId(keyPair.PublicKey);
            
            var password = $"ed:{hexSignature}:{publicKeyHex}";
            string deviceId = publicKeyHex;
            
            return new LoginRequest(new Uri(relayServer), hexId, password, deviceId);
        }
    }
}