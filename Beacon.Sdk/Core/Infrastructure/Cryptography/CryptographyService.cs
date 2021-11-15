namespace Beacon.Sdk.Core.Infrastructure.Cryptography
{
    using System;
    using System.Linq;
    using System.Text;
    using Domain.Interfaces.Data;
    using Libsodium;
    using Matrix.Sdk.Core.Utils;
    using Sodium;
    using SodiumCore = Sodium.SodiumCore;
    using SodiumLibrary = Libsodium.SodiumLibrary;

    public class CryptographyService //: ICryptographyService
        : ICryptographyService
    {
        private static readonly int MacBytes = SodiumLibrary.crypto_box_macbytes();
        private static readonly int NonceBytes = SodiumLibrary.crypto_box_noncebytes();

        public SessionKeyPair CreateServerSessionKeyPair(byte[] clientPublicKey, byte[] serverSecretKey)
        {
            byte[] serverPublicKeyCurve =
                PublicKeyAuth.ConvertEd25519PublicKeyToCurve25519PublicKey(serverSecretKey[32..64])!;
            byte[] serverSecretKeyCurve = PublicKeyAuth.ConvertEd25519SecretKeyToCurve25519SecretKey(serverSecretKey)!;
            byte[] clientPublicKeyCurve = PublicKeyAuth.ConvertEd25519PublicKeyToCurve25519PublicKey(clientPublicKey)!;

            return KeyExchange.CreateServerSessionKeyPair(serverPublicKeyCurve, serverSecretKeyCurve,
                clientPublicKeyCurve);
        }

        public byte[] Hash(byte[] input) => GenericHash.Hash(input, null, input.Length);

        public byte[] Decrypt(byte[] encryptedBytes, byte[] sharedKey)
        {
            byte[] nonce = encryptedBytes[..NonceBytes];
            byte[] cipher = encryptedBytes[NonceBytes..];

            return SecretBox.Open(cipher, nonce, sharedKey);
        }


        public byte[] Encrypt(byte[] message, byte[] recipientPublicKey)
        {
            byte[] nonce = SodiumCore.GetRandomBytes(NonceBytes)!;
            byte[] result = SealedPublicKeyBox.Create(message, recipientPublicKey);

            return nonce.Concat(result).ToArray();
        }

        public KeyPair GenerateEd25519KeyPair(string seed)
        {
            byte[] hash = GenericHash.Hash(seed, (byte[]?) null, 32);

            return PublicKeyAuth.GenerateKeyPair(hash);
        }

        public HexString EncryptAsHex(string message, byte[] recipientPublicKey)
        {
            byte[] bytes = HexString.TryParse(message, out HexString hexString)
                ? hexString.ToByteArray()
                : Encoding.UTF8.GetBytes(message);

            byte[] encryptedBytes = Encrypt(bytes, recipientPublicKey);

            if (!HexString.TryParse(encryptedBytes, out HexString result))
                throw new InvalidOperationException("Can not parse encryptedBytes");

            return result;
        }

        public string DecryptAsString(string encryptedMessage, byte[] sharedKey)
        {
            byte[] encryptedBytes = HexString.TryParse(encryptedMessage, out HexString hexString)
                ? hexString.ToByteArray()
                : Encoding.UTF8.GetBytes(encryptedMessage);

            byte[] decryptedBytes = Decrypt(encryptedBytes, sharedKey);

            return Encoding.UTF8.GetString(decryptedBytes);
        }

        public string ToHexString(byte[] input)
        {
            var hexString = BitConverter.ToString(input);
            string result = hexString.Replace("-", "");

            return result.ToLower();
        }

        public byte[] ConvertEd25519PublicKeyToCurve25519PublicKey(byte[] publicKey) =>
            PublicKeyAuth.ConvertEd25519PublicKeyToCurve25519PublicKey(publicKey) ??
            throw new NullReferenceException("Can not convert public key.");

        public string EncryptMessageAsString(string message, byte[] publicKey)
        {
            byte[]? result = SealedPublicKeyBox.Create(message, publicKey);

            return Encoding.UTF8.GetString(result);
        }

        public bool Validate(string input) =>
            HexString.TryParse(input,
                out HexString hexString) && // content can be non-hex if it's a connection open request
            hexString.ToString().Length >= NonceBytes + MacBytes;
        
        
        // public string ToHexString(byte[] input)
        // {
        //     var hexString = BitConverter.ToString(input);
        //
        //     string result = hexString.Replace("-", "");
        //
        //     return result.ToLower();
        // }

        public byte[] GenerateLoginDigest()
        {
            long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds() * 1000;
            var message = $"login:{now / 1000 / (5 * 60)}";

            return GenericHash.Hash(message, (byte[]?) null, 32);
        }

        // public KeyPair GenerateEd25519KeyPair(string seed)
        // {
        //     byte[] hash = GenericHash.Hash(seed, (byte[]?) null, 32);
        //
        //     return PublicKeyAuth.GenerateKeyPair(hash);
        // }

        public string GenerateHexSignature(byte[] loginDigest, byte[] secretKey)
        {
            byte[] signature = PublicKeyAuth.SignDetached(loginDigest, secretKey);

            return ToHexString(signature);
        }

        public string GenerateHexId(byte[] publicKey)
        {
            byte[] hash = GenericHash.Hash(publicKey, null, publicKey.Length);

            return ToHexString(hash);
        }
    }
}