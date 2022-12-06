namespace Beacon.Sdk.Core.Domain.Entities
{
    using System;
    using Infrastructure.Cryptography.Libsodium;
    using Interfaces;
    using Netezos.Encoding;
    using Utils;

    public class PeerFactory
    {
        private readonly ICryptographyService _cryptographyService;

        public PeerFactory(ICryptographyService cryptographyService)
        {
            _cryptographyService = cryptographyService;
        }

        public static byte[] Hash(byte[] message, int bufferLength)
        {
            var buffer = new byte[bufferLength];
            Sodium.CryptoGenericHash(buffer, bufferLength, message, message.Length, Array.Empty<byte>(), 0);

            return buffer;
        }

        public Peer Create(HexString hexPublicKey, string name, string version, string relayServer, bool isActive = false)
        {
            var hash = _cryptographyService.Hash(hexPublicKey.ToByteArray(), 5);
            var senderId = Base58.Convert(hash);

            return new Peer
            {
                SenderId = senderId,
                HexPublicKey = hexPublicKey,
                Name = name,
                Version = version,
                RelayServer = relayServer,
                IsActive = isActive
            };
        }
    }
}