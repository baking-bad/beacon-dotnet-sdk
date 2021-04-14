namespace BeaconSdk.ConsoleApp
{
    using System;
    using Sodium;

    public class CryptoService
    {
        // Todo: maybe use https://nsec.rocks/ ?
        private readonly ICryptoAlgorithmsProvider cryptoAlgorithmsProvider;

        public CryptoService(ICryptoAlgorithmsProvider cryptoAlgorithmsProvider)
        {
            this.cryptoAlgorithmsProvider = cryptoAlgorithmsProvider;
        }

        public byte[] GenerateLoginDigest()
        {
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds() * 1000;
            var message = $"login:{now / 1000 / (5 * 60)}";

            return cryptoAlgorithmsProvider.Hash(message, 32);
        }

        public KeyPair GenerateKeyPairFromSeed(string seed)
        {
            var hash = cryptoAlgorithmsProvider.Hash(seed, 32);

            return cryptoAlgorithmsProvider.GenerateEd25519KeyPair(hash);
        }

        public string GenerateHexSignature(byte[] loginDigest, byte[] secretKey)
        {
            var signature = cryptoAlgorithmsProvider.SignDetached(loginDigest, secretKey);

            return Convert.ToHexString(signature);
        }

        public string GenerateHexId(byte[] publicKey)
        {
            var hash = cryptoAlgorithmsProvider.Hash(publicKey, publicKey.Length);

            return Convert.ToHexString(hash);
        }
    }
}