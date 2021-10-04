namespace BeaconSdk.Infrastructure.Cryptography
{
    using System;
    using System.Linq;
    using System.Text;
    using Libsodium;
    using MatrixSdk.Utils;
    using Sodium;
    using SodiumCore = Sodium.SodiumCore;
    using SodiumLibrary = Libsodium.SodiumLibrary;

    public static class BeaconCryptographyService
    {
        private static readonly int MacBytes = SodiumLibrary.crypto_box_macbytes();
        private static readonly int NonceBytes = SodiumLibrary.crypto_box_noncebytes();

        public static bool Validate(string input) => HexString.TryParse(input, out var hexString) && hexString.ToString().Length >= NonceBytes + MacBytes;

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
            var cipher = encryptedBytes[NonceBytes..];

            return SecretBox.Open(cipher, nonce, sharedKey);
        }


        public static byte[] Encrypt(byte[] message, byte[] sharedKey)
        {
            var nonce = SodiumCore.GetRandomBytes(NonceBytes)!;
            var result = SecretBox.Create(message, nonce, sharedKey)!;

            return nonce.Concat(result).ToArray();
        }

        public static KeyPair GenerateEd25519KeyPair(string seed)
        {
            var hash = GenericHash.Hash(seed, (byte[]?)null, 32);

            return PublicKeyAuth.GenerateKeyPair(hash);
        }

        public static HexString EncryptAsHex(string message, byte[] sharedKey)
        {
            var bytes = HexString.TryParse(message, out var hexString)
                ? hexString.ToByteArray()
                : Encoding.UTF8.GetBytes(message);

            var encryptedBytes = Encrypt(bytes, sharedKey);

            if (!HexString.TryParse(encryptedBytes, out var result))
                throw new InvalidOperationException("Can not parse encryptedBytes");

            return result;
        }

        public static string DecryptAsString(string encryptedMessage, byte[] sharedKey)
        {
            var encryptedBytes = HexString.TryParse(encryptedMessage, out var hexString)
                ? hexString.ToByteArray()
                : Encoding.UTF8.GetBytes(encryptedMessage);

            var decryptedBytes = Decrypt(encryptedBytes, sharedKey);

            return Encoding.UTF8.GetString(decryptedBytes);
        }
    }
}