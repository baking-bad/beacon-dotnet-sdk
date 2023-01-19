namespace Beacon.Sdk.Core.Infrastructure.Cryptography
{
    using System;
    using System.Linq;
    using System.Text;
    using Domain.Interfaces;
    using Libsodium;
    using Utils;
    using Sodium = Libsodium.Sodium;

    public class CryptographyService : ICryptographyService
    {
        private static readonly int MacBytes = Sodium.CryptoBoxMacBytes();
        private static readonly int NonceBytes = Sodium.CryptoBoxNonceBytes();

        public SessionKeyPair CreateClientSessionKeyPair(byte[] clientPublicKey, byte[] serverPrivateKey)
        {
            byte[] serverPublicKeyCurve =
                PublicKeyAuth.ConvertEd25519PublicKeyToCurve25519PublicKey(serverPrivateKey[32..64])!;
            byte[] serverSecretKeyCurve = PublicKeyAuth.ConvertEd25519SecretKeyToCurve25519SecretKey(serverPrivateKey)!;
            byte[] clientPublicKeyCurve = PublicKeyAuth.ConvertEd25519PublicKeyToCurve25519PublicKey(clientPublicKey)!;

            return KeyExchange.CreateClientSessionKeyPair(serverPublicKeyCurve, serverSecretKeyCurve,
                clientPublicKeyCurve);
        }

        public SessionKeyPair CreateServerSessionKeyPair(byte[] clientPublicKey, byte[] serverPrivateKey)
        {
            byte[] serverPublicKeyCurve =
                PublicKeyAuth.ConvertEd25519PublicKeyToCurve25519PublicKey(serverPrivateKey[32..64])!;
            byte[] serverSecretKeyCurve = PublicKeyAuth.ConvertEd25519SecretKeyToCurve25519SecretKey(serverPrivateKey)!;
            byte[] clientPublicKeyCurve = PublicKeyAuth.ConvertEd25519PublicKeyToCurve25519PublicKey(clientPublicKey)!;

            return KeyExchange.CreateServerSessionKeyPair(serverPublicKeyCurve, serverSecretKeyCurve,
                clientPublicKeyCurve);
        }

        public byte[] Hash(byte[] input) => GenericHash.Hash(input, null, input.Length);

        public byte[] Hash(byte[] message, int bufferLength)
        {
            var buffer = new byte[bufferLength];

            Sodium.Initialize();
            Sodium.CryptoGenericHash(buffer, bufferLength, message, (ulong)message.Length, Array.Empty<byte>(), 0);
 
            return buffer;
        }

        public KeyPair GenerateEd25519KeyPair(string seed)
        {
            byte[] hash = GenericHash.Hash(seed, (byte[]?)null, 32);

            return PublicKeyAuth.GenerateKeyPair(hash);
        }

        public HexString Encrypt(string input, byte[] key)
        {
            byte[] nonce = SecureRandom.GetRandomBytes(NonceBytes)!;
            byte[] e = SecretBox.Create(input, nonce, key);

            byte[] payload = nonce.Concat(e).ToArray();

            if (!HexString.TryParse(payload, out HexString hexPayload))
                throw new ArgumentException(nameof(payload));

            return hexPayload;
        }

        // public static class ArrayExtensions
        // {
        //     public byte this[Range input]
        //     {
        //         get;
        //         set;
        //     }
        // }
        public string Decrypt(HexString hexInput, byte[] key)
        {
            byte[] bytes = hexInput.ToByteArray();

            byte[] nonce = bytes[..NonceBytes];
            byte[] cipher = bytes[NonceBytes..];

            byte[] d = SecretBox.Open(cipher, nonce, key);

            return Encoding.UTF8.GetString(d);
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

        public byte[] ConvertEd25519SecretKeyToCurve25519SecretKey(byte[] secretKey) =>
            PublicKeyAuth.ConvertEd25519SecretKeyToCurve25519SecretKey(secretKey) ??
            throw new NullReferenceException("Can not convert secret key.");

        public string EncryptMessageAsString(string message, byte[] publicKey)
        {
            byte[] encrypted = SealedPublicKeyBox.Create(message, publicKey);

            if (!HexString.TryParse(encrypted, out HexString encryptedHex))
                throw new Exception("HexString.TryParse(result, out var k)");

            return encryptedHex.Value;
        }

        public string DecryptMessageAsString(string message, HexString secretKey, HexString publicKey)
        {
            if (!HexString.TryParse(message, out HexString encryptedMessageHex))
                throw new Exception("Can't parse message in DecryptMessageAsString");

            byte[] curve25519PublicKey = ConvertEd25519PublicKeyToCurve25519PublicKey(publicKey.ToByteArray());
            byte[] curve25519SecretKey = ConvertEd25519SecretKeyToCurve25519SecretKey(secretKey.ToByteArray());
            byte[] decrypted = SealedPublicKeyBox.Open(encryptedMessageHex.ToByteArray(), curve25519SecretKey,
                curve25519PublicKey);

            return Encoding.UTF8.GetString(decrypted);
        }

        public bool Validate(string input) =>
            HexString.TryParse(input,
                out HexString hexString) && // content can be non-hex if it's a connection open request
            hexString.ToString().Length >= NonceBytes + MacBytes;

        public byte[] GenerateLoginDigest()
        {
            long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds() * 1000;
            var message = $"login:{now / 1000 / (5 * 60)}";

            return GenericHash.Hash(message, (byte[]?)null, 32);
        }

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