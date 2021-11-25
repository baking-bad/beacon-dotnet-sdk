namespace Beacon.Sdk.Core.Domain.Services
{
    using System.Text;
    using Base58Check;
    using Infrastructure.Cryptography.Libsodium;
    using Interfaces;
    using Interfaces.Data;
    using Utils;

    public class P2PMessageService
    {
        private readonly ICryptographyService _cryptographyService;
        private readonly ISessionKeyPairRepository _sessionKeyPairRepository;
        private readonly KeyPairService _keyPairService;

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
            SessionKeyPair server = _sessionKeyPairRepository.CreateOrReadServer(peerHexPublicKey, _keyPairService.KeyPair);

            string decrypt = _cryptographyService.Decrypt(hexMessage, server.Rx);
            byte[] decode = Base58CheckEncoding.Decode(decrypt)!;
            
            return Encoding.UTF8.GetString(decode);
        }

        public HexString EncryptMessage(HexString peerHexPublicKey, string message)
        {
            SessionKeyPair client = _sessionKeyPairRepository.CreateOrReadClient(peerHexPublicKey, _keyPairService.KeyPair);

            byte[] bytes = Encoding.UTF8.GetBytes(message);
            string encode = Base58CheckEncoding.Encode(bytes);
            
            return _cryptographyService.Encrypt(encode, client.Tx);
        }
    }
}