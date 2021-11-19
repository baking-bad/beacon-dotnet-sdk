namespace Beacon.Sdk.Core.Transport.P2P
{
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using Base58Check;
    using Domain;
    using Domain.Interfaces.Data;
    using Infrastructure.Cryptography.Libsodium;
    using Matrix.Sdk.Core.Domain.RoomEvent;
    using Microsoft.Extensions.Logging;
    using Sodium;

    public class SessionCryptographyService
    {
        private readonly IKeyPairRepository _keyPairRepository;
        private readonly IBeaconPeerRepository _beaconPeerRepository;
        private readonly ICryptographyService _cryptographyService;
        private readonly RelayServerService _relayServerService;
        private readonly ILogger<SessionCryptographyService>? _logger;

        public SessionCryptographyService(
            IKeyPairRepository keyPairRepository,
            IBeaconPeerRepository beaconPeerRepository,
            ICryptographyService cryptographyService,
            RelayServerService relayServerService,
            ILogger<SessionCryptographyService>? logger)
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
                _logger?.LogInformation("Unknown sender");
                return null;
            }

            if (!senderUserId.StartsWith(peer.UserId)) return null;

            if (!_cryptographyService.Validate(textMessageEvent.Message))
            {
                _logger?.LogInformation("Can not validate message");
                return null;
            }

            KeyPair keyPair =  _keyPairRepository.KeyPair;

            SessionKeyPair serverSessionKeyPair =
                _keyPairRepository.CreateOrReadServerSession(peer.HexPublicKey, keyPair);

            string decryptedMessage = _cryptographyService.DecryptAsString(textMessageEvent.Message,
                serverSessionKeyPair.Rx);
            
            byte[]? decodedBytes = Base58CheckEncoding.Decode(decryptedMessage);
            return Encoding.UTF8.GetString(decodedBytes);
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
           
            var address = new Uri($@"https://{relayServer}");
            
            return new LoginRequest(address, hexId, password, deviceId);
        }

        public record LoginRequest(Uri Address, string Username, string Password, string DeviceId);

    }
}