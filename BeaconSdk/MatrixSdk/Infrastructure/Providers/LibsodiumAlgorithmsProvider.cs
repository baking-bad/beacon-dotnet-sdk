namespace MatrixSdk.Infrastructure.Providers
{
    using System;
    using Sodium;
    using Utils;

    public class LibsodiumAlgorithmsProvider : ICryptoAlgorithmsProvider
    {
        // ToDo: implement 
        /*
         * func validate(encrypted: String) -> Bool {
                do {
                    return try HexString(from: encrypted).count() >= crypto_box_noncebytes() + crypto_box_macbytes() // 
                } catch {
                    return false
                }
            }
         */
        public bool Validate(string input) => true;

        public byte[] GenerateRandomBytes(int count) => SodiumCore.GetRandomBytes(count);

        public byte[] Hash(string message, int size) => GenericHash.Hash(message, (byte[]?)null, size);

        public byte[] Hash(byte[] message, int size) => GenericHash.Hash(message, null, size);

        public KeyPair GenerateEd25519KeyPair(byte[] seed) => PublicKeyAuth.GenerateKeyPair(seed);

        public byte[] SignDetached(byte[] message, byte[] key) => PublicKeyAuth.SignDetached(message, key);

        public void CreateServerSessionKeyPair(Span<byte> serverPublicKey, Span<byte> serverSecretKey, Span<byte> clientPublicKey)
        {
        }
    }
}