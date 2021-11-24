namespace Beacon.Sdk.Core.Domain.Services
{
    using System.Text;
    using Base58Check;
    using Interfaces;
    using Interfaces.Data;
    using Infrastructure.Cryptography.Libsodium;
    using Matrix.Sdk.Core.Domain.RoomEvent;
    using Microsoft.Extensions.Logging;
    using Sodium;

    public class P2PMessageService
    {
        private readonly IBeaconPeerRepository _beaconPeerRepository;

        private readonly ICryptographyService _cryptographyService;

        private readonly KeyPairService _keyPairService;
        private readonly ILogger<P2PMessageService> _logger;
        private readonly ISessionKeyPairRepository _sessionKeyPairRepository;

        public P2PMessageService(
            ILogger<P2PMessageService> logger,
            ICryptographyService cryptographyService,
            IBeaconPeerRepository beaconPeerRepository,
            ISessionKeyPairRepository sessionKeyPairRepository,
            KeyPairService keyPairService)
        {
            _logger = logger;
            _cryptographyService = cryptographyService;
            _beaconPeerRepository = beaconPeerRepository;
            _sessionKeyPairRepository = sessionKeyPairRepository;
            _keyPairService = keyPairService;
        }


        public string? TryDecryptMessageFromEvent(BaseRoomEvent matrixRoomEvent)
        {
            if (matrixRoomEvent is not TextMessageEvent textMessageEvent) return null;

            string senderUserId = textMessageEvent.SenderUserId;
            BeaconPeer? peer = _beaconPeerRepository.TryReadByUserId(senderUserId).Result;

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

            KeyPair keyPair = _keyPairService.KeyPair;

            SessionKeyPair serverSessionKeyPair =
                _sessionKeyPairRepository.CreateOrReadServer(peer.HexPublicKey, keyPair);

            string decryptedMessage = _cryptographyService.DecryptAsString(textMessageEvent.Message,
                serverSessionKeyPair.Rx);

            byte[]? decodedBytes = Base58CheckEncoding.Decode(decryptedMessage);
            return Encoding.UTF8.GetString(decodedBytes);
        }
    }
}