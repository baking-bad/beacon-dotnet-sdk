namespace Beacon.Sdk.Core.Domain.Services
{
    using System.Text;
    using Base58Check;
    using Infrastructure.Cryptography.Libsodium;
    using Interfaces;
    using Interfaces.Data;
    using Matrix.Sdk.Core.Domain.RoomEvent;
    using Microsoft.Extensions.Logging;
    using Sodium;
    using Utils;

    public class P2PMessageService
    {
        private readonly IPeerRepository _peerRepository;

        private readonly ICryptographyService _cryptographyService;

        private readonly KeyPairService _keyPairService;
        private readonly ILogger<P2PMessageService> _logger;
        private readonly ISessionKeyPairRepository _sessionKeyPairRepository;

        public P2PMessageService(
            ILogger<P2PMessageService> logger,
            ICryptographyService cryptographyService,
            IPeerRepository peerRepository,
            ISessionKeyPairRepository sessionKeyPairRepository,
            KeyPairService keyPairService)
        {
            _logger = logger;
            _cryptographyService = cryptographyService;
            _peerRepository = peerRepository;
            _sessionKeyPairRepository = sessionKeyPairRepository;
            _keyPairService = keyPairService;
        }


        public string? TryDecryptMessageFromEvent(BaseRoomEvent matrixRoomEvent)
        {
            if (matrixRoomEvent is not TextMessageEvent textMessageEvent) return null;

            string senderUserId = textMessageEvent.SenderUserId;
            Peer? peer = _peerRepository.TryReadByUserId(senderUserId).Result;

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

            SessionKeyPair server = _sessionKeyPairRepository.CreateOrReadServer(peer.HexPublicKey, _keyPairService.KeyPair);

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