namespace Beacon.Sdk.Core.Domain.Services.P2P
{
    using System.Text;
    using Base58Check;
    using Infrastructure.Cryptography.Libsodium;
    using Interfaces;
    using Interfaces.Data;
    using Matrix.Sdk.Core.Domain.RoomEvent;
    using Microsoft.Extensions.Logging;
    using Sodium;

    public class MessageService
    {
        private readonly IBeaconPeerRepository _beaconPeerRepository;

        private readonly ICryptographyService _cryptographyService;

        private readonly KeyPairService _keyPairService;
        private readonly ILogger<MessageService> _logger;
        private readonly ISessionKeyPairRepository _sessionKeyPairRepository;

        public MessageService(
            ILogger<MessageService> logger,
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