namespace Beacon.Sdk.Core.Infrastructure.Cryptography
{
    using System;
    using System.Linq;
    using System.Text;
    using Libsodium;
    using Matrix.Sdk.Core.Utils;
    using Sodium;
    using SodiumCore = Sodium.SodiumCore;
    using SodiumLibrary = Libsodium.SodiumLibrary;

    public static class BeaconCryptographyService
    {
        private static readonly int MacBytes = SodiumLibrary.crypto_box_macbytes();
        private static readonly int NonceBytes = SodiumLibrary.crypto_box_noncebytes();

        public static bool Validate(string input) => HexString.TryParse(input, out HexString hexString) &&
                                                     hexString.ToString().Length >= NonceBytes + MacBytes;

        public static SessionKeyPair CreateServerSessionKeyPair(byte[] clientPublicKey, byte[] serverSecretKey)
        {
            byte[] serverPublicKeyCurve =
                PublicKeyAuth.ConvertEd25519PublicKeyToCurve25519PublicKey(serverSecretKey[32..64])!;
            byte[] serverSecretKeyCurve = PublicKeyAuth.ConvertEd25519SecretKeyToCurve25519SecretKey(serverSecretKey)!;
            byte[] clientPublicKeyCurve = PublicKeyAuth.ConvertEd25519PublicKeyToCurve25519PublicKey(clientPublicKey)!;

            return KeyExchange.CreateServerSessionKeyPair(serverPublicKeyCurve, serverSecretKeyCurve,
                clientPublicKeyCurve);
        }

        public static byte[] Hash(byte[] input) => GenericHash.Hash(input, null, input.Length);

        public static byte[] Decrypt(byte[] encryptedBytes, byte[] sharedKey)
        {
            byte[] nonce = encryptedBytes[..NonceBytes];
            byte[] cipher = encryptedBytes[NonceBytes..];

            return SecretBox.Open(cipher, nonce, sharedKey);
        }


        public static byte[] Encrypt(byte[] message, byte[] recipientPublicKey)
        {
            byte[] nonce = SodiumCore.GetRandomBytes(NonceBytes)!;
            byte[] result = SealedPublicKeyBox.Create(message, recipientPublicKey);

            return nonce.Concat(result).ToArray();
        }

        public static KeyPair GenerateEd25519KeyPair(string seed)
        {
            byte[] hash = GenericHash.Hash(seed, (byte[]?) null, 32);

            return PublicKeyAuth.GenerateKeyPair(hash);
        }

        public static HexString EncryptAsHex(string message, byte[] recipientPublicKey)
        {
            byte[] bytes = HexString.TryParse(message, out HexString hexString)
                ? hexString.ToByteArray()
                : Encoding.UTF8.GetBytes(message);

            byte[] encryptedBytes = Encrypt(bytes, recipientPublicKey);

            if (!HexString.TryParse(encryptedBytes, out HexString result))
                throw new InvalidOperationException("Can not parse encryptedBytes");

            return result;
        }

        public static string DecryptAsString(string encryptedMessage, byte[] sharedKey)
        {
            byte[] encryptedBytes = HexString.TryParse(encryptedMessage, out HexString hexString)
                ? hexString.ToByteArray()
                : Encoding.UTF8.GetBytes(encryptedMessage);

            byte[] decryptedBytes = Decrypt(encryptedBytes, sharedKey);

            return Encoding.UTF8.GetString(decryptedBytes);
        }

        public static string ToHexString(byte[] input)
        {
            var hexString = BitConverter.ToString(input);
            string result = hexString.Replace("-", "");

            return result.ToLower();
        }
    }
}