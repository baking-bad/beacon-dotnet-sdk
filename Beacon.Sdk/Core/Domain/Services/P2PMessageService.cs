namespace Beacon.Sdk.Core.Domain.Services
{
    using System.Text;
    using global::Beacon.Sdk.Core.Infrastructure.Cryptography;
    using Interfaces;
    using Interfaces.Data;
    using Netezos.Encoding;
    using Utils;

    public class P2PMessageService
    {
        private readonly ICryptographyService _cryptographyService;
        private readonly KeyPairService _keyPairService;
        private readonly ISessionKeyPairRepository _sessionKeyPairRepository;

        public P2PMessageService(ICryptographyService cryptographyService,
            ISessionKeyPairRepository sessionKeyPairRepository,
            KeyPairService keyPairService)
        {
            _cryptographyService = cryptographyService;
            _sessionKeyPairRepository = sessionKeyPairRepository;
            _keyPairService = keyPairService;
        }

        public string DecryptMessage(HexString peerHexPublicKey, HexString hexMessage)
        {
            SessionKeyPair server =
                _sessionKeyPairRepository.CreateOrReadServer(peerHexPublicKey, _keyPairService.KeyPair);

            string decrypt = _cryptographyService.Decrypt(hexMessage, server.Rx);
            byte[] decode = Base58.Parse(decrypt);

            return Encoding.UTF8.GetString(decode);
        }

        public HexString EncryptMessage(HexString peerHexPublicKey, string message)
        {
            SessionKeyPair client =
                _sessionKeyPairRepository.CreateOrReadClient(peerHexPublicKey, _keyPairService.KeyPair);

            byte[] bytes = Encoding.UTF8.GetBytes(message);
            string encode = Base58.Convert(bytes);

            return _cryptographyService.Encrypt(encode, client.Tx);
        }
    }
}