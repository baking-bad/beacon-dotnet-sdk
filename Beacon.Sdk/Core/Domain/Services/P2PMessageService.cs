namespace Beacon.Sdk.Core.Domain.Services
{
    using System.Text;
    using Base58Check;
    using Infrastructure.Cryptography.Libsodium;
    using Interfaces;
    using Interfaces.Data;
    using Matrix.Sdk.Core.Domain.RoomEvent;
    using Microsoft.Extensions.Logging;
    using P2P;
    using Utils;

    public class P2PMessageService
    {
        private readonly ILogger<P2PMessageService> _logger;
        private readonly IP2PPeerRoomRepository _p2PPeerRoomRepository;
        private readonly ICryptographyService _cryptographyService;
        private readonly ISessionKeyPairRepository _sessionKeyPairRepository;
        private readonly KeyPairService _keyPairService;

        public P2PMessageService(
            ILogger<P2PMessageService> logger, 
            IP2PPeerRoomRepository p2PPeerRoomRepository,
            ICryptographyService cryptographyService, 
            ISessionKeyPairRepository sessionKeyPairRepository, 
            KeyPairService keyPairService)
        {
            _logger = logger;
            _p2PPeerRoomRepository = p2PPeerRoomRepository;
            _cryptographyService = cryptographyService;
            _sessionKeyPairRepository = sessionKeyPairRepository;
            _keyPairService = keyPairService;
        }

        public string? TryDecryptMessageFromEvent(BaseRoomEvent matrixRoomEvent)
        {
            if (matrixRoomEvent is not TextMessageEvent textMessageEvent) return null;

            string senderUserId = textMessageEvent.SenderUserId;
            P2PPeerRoom? p2PPeerRoom = _p2PPeerRoomRepository.TryReadByP2PUserId(senderUserId).Result;
            
            if (p2PPeerRoom == null)
            {
                _logger?.LogInformation("Unknown sender");
                return null;
            }

            if (!_cryptographyService.Validate(textMessageEvent.Message))
            {
                _logger?.LogInformation("Can not validate message");
                return null;
            }

            SessionKeyPair server = _sessionKeyPairRepository.CreateOrReadServer(p2PPeerRoom.PeerHexPublicKey, _keyPairService.KeyPair);

            string decryptedMessage = _cryptographyService.DecryptAsString(textMessageEvent.Message, server.Rx);

            byte[]? decodedBytes = Base58CheckEncoding.Decode(decryptedMessage);
            return Encoding.UTF8.GetString(decodedBytes);
        }

        public string EncryptMessage(HexString peerHexPublicKey, string message)
        {
            SessionKeyPair client = _sessionKeyPairRepository.CreateOrReadClient(peerHexPublicKey, _keyPairService.KeyPair);

            return _cryptographyService.EncryptAsHex(message, client.Tx).Value;
        }
    }
}

// Peer? peer = _peerRepository.TryReadBySenderUserId(senderUserId).Result;
// if (!senderUserId.StartsWith(p2PPeerRoom.P2PUserId)) return null;