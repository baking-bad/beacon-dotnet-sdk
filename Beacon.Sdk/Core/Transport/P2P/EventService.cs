namespace Beacon.Sdk.Core.Transport.P2P
{
    using Domain;
    using Domain.Interfaces.Data;
    using Infrastructure.Cryptography.Libsodium;
    using Matrix.Sdk.Core.Domain.Room;
    using Microsoft.Extensions.Logging;
    using Sodium;

    public class EventService
    {
        private readonly ISessionKeyPairRepository _sessionKeyPairRepository;
        private readonly IBeaconPeerRepository _beaconPeerRepository;
        private readonly ICryptographyService _cryptographyService;
        private readonly ILogger<EventService> _logger;

        public EventService(
            ISessionKeyPairRepository sessionKeyPairRepository, 
            IBeaconPeerRepository beaconPeerRepository, 
            ICryptographyService cryptographyService, 
            ILogger<EventService> logger)
        {
            _sessionKeyPairRepository = sessionKeyPairRepository;
            _beaconPeerRepository = beaconPeerRepository;
            _cryptographyService = cryptographyService;
            _logger = logger;
        }

        public string? TryDecryptMessageFromEvent(BaseRoomEvent matrixRoomEvent, KeyPair keyPair)
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

            SessionKeyPair serverSessionKeyPair =
                _sessionKeyPairRepository.CreateOrReadServer(peer.HexPublicKey, keyPair);

            return _cryptographyService.DecryptAsString(textMessageEvent.Message,
                serverSessionKeyPair.Rx);
        }
    }
}