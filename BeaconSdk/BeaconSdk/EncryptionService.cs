namespace BeaconSdk
{
    using Libsodium;
    using MatrixSdk.Utils;
    using Sodium;
    using SodiumLibrary = Libsodium.SodiumLibrary;

    public static class EncryptionService
    {
        private static readonly int MacBytes = SodiumLibrary.crypto_box_macbytes();
        private static readonly int NonceBytes = SodiumLibrary.crypto_box_noncebytes();

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
        public static bool Validate(string input) => HexString.TryParse(input, out var hexString) && hexString.ToString().Length >= MacBytes + NonceBytes;

        public static SessionKeyPair CreateServerSessionKeyPair(byte[] clientPublicKey, byte[] serverSecretKey)
        {
            var serverPublicKeyCurve = PublicKeyAuth.ConvertEd25519PublicKeyToCurve25519PublicKey(serverSecretKey[32..64])!;
            var serverSecretKeyCurve = PublicKeyAuth.ConvertEd25519SecretKeyToCurve25519SecretKey(serverSecretKey)!;
            var clientPublicKeyCurve = PublicKeyAuth.ConvertEd25519PublicKeyToCurve25519PublicKey(clientPublicKey)!;

            return KeyExchange.CreateServerSessionKeyPair(serverPublicKeyCurve, serverSecretKeyCurve, clientPublicKeyCurve);
        }

        public static byte[] Hash(byte[] input) => GenericHash.Hash(input, null, input.Length);

        public static byte[] Decrypt(byte[] encryptedBytes, byte[] sharedKey)
        {
            var nonce = encryptedBytes[..NonceBytes];
            var cypherText = encryptedBytes[NonceBytes..];

            return SecretBox.Open(cypherText, nonce, sharedKey);
        }
        
        // public static byte[] EncryptWithSharedKey(byte[] sharedKey, byte[] message)
        // {
        //     
        // }

        // public byte[] Hash(byte[] message, int size) => GenericHash.Hash(message, null, size);

        // public byte[] GenerateRandomBytes(int count) => SodiumCore.GetRandomBytes(count);
    }
}