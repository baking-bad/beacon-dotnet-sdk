namespace MatrixSdk.Infrastructure.Providers
{
    using Sodium;

    internal class LibsodiumAlgorithmsProvider : ICryptoAlgorithmsProvider
    {
        public byte[] GenerateRandomBytes(int count) => SodiumCore.GetRandomBytes(count);

        public byte[] Hash(string message, int size) => GenericHash.Hash(message, (byte[]?) null, size);

        public byte[] Hash(byte[] message, int size) => GenericHash.Hash(message, null, size);

        public KeyPair GenerateEd25519KeyPair(byte[] seed) => PublicKeyAuth.GenerateKeyPair(seed);

        public byte[] SignDetached(byte[] message, byte[] key) => PublicKeyAuth.SignDetached(message, key);
    }
}