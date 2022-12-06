using System;

using Beacon.Sdk.Core.Infrastructure.Cryptography.Libsodium;

namespace Beacon.Sdk.Core.Infrastructure.Cryptography
{
    /// <summary>Public-key signatures</summary>
    public static class PublicKeyAuth
    {
        private const int SECRET_KEY_BYTES = Sodium.crypto_sign_ed25519_SECRETKEYBYTES;
        private const int PUBLIC_KEY_BYTES = Sodium.crypto_sign_ed25519_PUBLICKEYBYTES;
        private const int BYTES = Sodium.crypto_sign_ed25519_BYTES;
        private const int SEED_BYTES = Sodium.crypto_sign_ed25519_SEEDBYTES;

        public static int SecretKeyBytes { get; } = SECRET_KEY_BYTES;
        public static int PublicKeyBytes { get; } = PUBLIC_KEY_BYTES;
        public static int SignatureBytes { get; } = BYTES;
        public static int SeedBytes { get; } = SEED_BYTES;

        /// <summary>Creates a new key pair based on the provided seed.</summary>
        /// <param name="seed">The seed.</param>
        /// <returns>A KeyPair.</returns>
        /// <exception cref="SeedOutOfRangeException"></exception>
        public static KeyPair GenerateKeyPair(byte[] seed)
        {
            if (seed == null || seed.Length != SEED_BYTES)
                throw new ArgumentOutOfRangeException(nameof(seed), seed?.Length ?? 0, $"seed must be {SEED_BYTES} bytes in length.");

            var publicKey = new byte[PUBLIC_KEY_BYTES];
            var privateKey = new byte[SECRET_KEY_BYTES];

            Sodium.Initialize();
            Sodium.CryptoSignEd25519SeedKeyPair(publicKey, privateKey, seed);

            return new KeyPair(publicKey, privateKey);
        }

        /// <summary>Signs a message with Ed25519.</summary>
        /// <param name="message">The message.</param>
        /// <param name="key">The 64 byte private key.</param>
        /// <returns>Signed message.</returns>
        /// <exception cref="KeyOutOfRangeException"></exception>
        public static byte[] Sign(byte[] message, byte[] key)
        {
            if (key == null || key.Length != SECRET_KEY_BYTES)
                throw new ArgumentOutOfRangeException(nameof(key), key?.Length ?? 0, $"key must be {SECRET_KEY_BYTES} bytes in length.");

            var buffer = new byte[message.Length + BYTES];
            ulong bufferLength = 0;

            Sodium.Initialize();
            Sodium.CryptoSignEd25519(buffer, ref bufferLength, message, (ulong)message.Length, key);

            Array.Resize(ref buffer, (int)bufferLength);
            return buffer;
        }

        /// <summary>Signs a message with Ed25519.</summary>
        /// <param name="message">The message.</param>
        /// <param name="key">The 64 byte private key.</param>
        /// <returns>The signature.</returns>
        /// <exception cref="KeyOutOfRangeException"></exception>
        public static byte[] SignDetached(byte[] message, byte[] key)
        {
            if (key == null || key.Length != SECRET_KEY_BYTES)
                throw new ArgumentOutOfRangeException(nameof(key), key?.Length ?? 0, $"key must be {SECRET_KEY_BYTES} bytes in length.");

            var signature = new byte[BYTES];
            ulong signatureLength = 0;

            Sodium.Initialize();
            Sodium.CryptoSignEd25519Detached(signature, ref signatureLength, message, (ulong)message.Length, key);

            return signature;
        }

        /// <summary>Converts the ed25519 public key to curve25519 public key.</summary>
        /// <param name="ed25519PublicKey">Ed25519 public key.</param>
        /// <returns>The curve25519 public key.</returns>
        /// <exception cref="KeyOutOfRangeException"></exception>
        /// <exception cref="CryptographicException"></exception>
        public static byte[] ConvertEd25519PublicKeyToCurve25519PublicKey(byte[] ed25519PublicKey)
        {
            if (ed25519PublicKey == null || ed25519PublicKey.Length != PUBLIC_KEY_BYTES)
                throw new ArgumentOutOfRangeException(nameof(ed25519PublicKey), ed25519PublicKey?.Length ?? 0, $"ed25519PublicKey must be {PUBLIC_KEY_BYTES} bytes in length.");

            var buffer = new byte[Sodium.crypto_scalarmult_curve25519_BYTES];

            Sodium.Initialize();
            var ret = Sodium.CryptoSignEd25519PkToCurve25519(buffer, ed25519PublicKey);

            if (ret != 0)
                throw new Exception("Failed to convert public key.");

            return buffer;
        }

        /// <summary>Converts the ed25519 secret key to curve25519 secret key.</summary>
        /// <param name="ed25519SecretKey">Ed25519 secret key.</param>
        /// <returns>The curve25519 secret key.</returns>
        /// <exception cref="KeyOutOfRangeException"></exception>
        /// <exception cref="CryptographicException"></exception>
        public static byte[] ConvertEd25519SecretKeyToCurve25519SecretKey(byte[] ed25519SecretKey)
        {
            // key can be appended with the public key or not (both are allowed)
            if (ed25519SecretKey == null || (ed25519SecretKey.Length != PUBLIC_KEY_BYTES && ed25519SecretKey.Length != SECRET_KEY_BYTES))
                throw new ArgumentOutOfRangeException(nameof(ed25519SecretKey), ed25519SecretKey?.Length ?? 0, $"ed25519SecretKey must be either {PUBLIC_KEY_BYTES} or {SECRET_KEY_BYTES} bytes in length.");

            var buffer = new byte[Sodium.crypto_scalarmult_curve25519_SCALARBYTES];

            Sodium.Initialize();
            var ret = Sodium.CryptoSignEd25519SkToCurve25519(buffer, ed25519SecretKey);

            if (ret != 0)
                throw new Exception("Failed to convert secret key.");

            return buffer;
        }
    }
}
