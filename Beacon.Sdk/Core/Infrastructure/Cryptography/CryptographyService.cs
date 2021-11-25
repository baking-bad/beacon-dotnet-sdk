namespace Beacon.Sdk.Core.Infrastructure.Cryptography
{
    using System;
    using System.Linq;
    using System.Text;
    using Domain.Interfaces;
    using Libsodium;
    using Sodium;
    using Utils;
    using SodiumLibrary = global::Beacon.Sdk.Core.Infrastructure.Cryptography.Libsodium.SodiumLibrary;

    public class CryptographyService : ICryptographyService
    {
        private static readonly int MacBytes = SodiumLibrary.crypto_box_macbytes();
        private static readonly int NonceBytes = SodiumLibrary.crypto_box_noncebytes();

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

        public KeyPair GenerateEd25519KeyPair(string seed)
        {
            byte[] hash = GenericHash.Hash(seed, (byte[]?) null, 32);

            return PublicKeyAuth.GenerateKeyPair(hash);
        }

        public HexString Encrypt(string input, byte[] key)
        {
            byte[] nonce = SodiumCore.GetRandomBytes(NonceBytes)!;
            byte[]
                e = SecretBox.Create(input, nonce,
                    key); //SealedPublicKeyBox.Create(String.Concat(nonce).ToString(), key);

            byte[] payload = nonce.Concat(e).ToArray();

            if (!HexString.TryParse(payload, out HexString hexPayload))
                throw new ArgumentException(nameof(payload));

            return hexPayload;
        }

        public string Decrypt(HexString hexInput, byte[] key)
        {
            byte[] bytes = hexInput.ToByteArray();

            byte[] nonce = bytes[..NonceBytes];
            byte[] cipher = bytes[NonceBytes..];

            byte[] d = SecretBox.Open(cipher, nonce, key);

            return Encoding.UTF8.GetString(d);
        }

        // private byte[] Decrypt2(byte[] encryptedBytes, byte[] sharedKey)
        // {
        //     byte[] nonce = encryptedBytes[..NonceBytes];
        //     byte[] cipher = encryptedBytes[NonceBytes..];
        //
        //     return SecretBox.Open(cipher, nonce, sharedKey);
        // }

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

            if (!HexString.TryParse(result, out HexString k))
                throw new Exception("HexString.TryParse(result, out var k)");

            return k.Value;
        }

        public bool Validate(string input) =>
            HexString.TryParse(input,
                out HexString hexString) && // content can be non-hex if it's a connection open request
            hexString.ToString().Length >= NonceBytes + MacBytes;

        public byte[] GenerateLoginDigest()
        {
            long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds() * 1000;
            var message = $"login:{now / 1000 / (5 * 60)}";

            return GenericHash.Hash(message, (byte[]?) null, 32);
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