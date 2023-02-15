using System;

using Beacon.Sdk.Core.Infrastructure.Cryptography.BouncyCastle;
using Org.BouncyCastle.Math.EC.Rfc8032;

namespace Beacon.Sdk.Core.Infrastructure.Cryptography
{
    /// <summary>Public-key signatures</summary>
    public static class PublicKeyAuth
    {
        private const int SECRET_KEY_BYTES = 32 + 32;
        private const int PUBLIC_KEY_BYTES = 32;
        private const int BYTES = 64;
        private const int SEED_BYTES = 32;

        public static int SecretKeyBytes { get; } = SECRET_KEY_BYTES;
        public static int PublicKeyBytes { get; } = PUBLIC_KEY_BYTES;
        public static int SignatureBytes { get; } = BYTES;
        public static int SeedBytes { get; } = SEED_BYTES;

        /// <summary>Creates a new key pair based on the provided seed.</summary>
        /// <param name="seed">The seed.</param>
        /// <returns>A KeyPair.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static KeyPair GenerateKeyPair(byte[] seed)
        {
            if (seed == null || seed.Length != SEED_BYTES)
                throw new ArgumentOutOfRangeException(nameof(seed), seed?.Length ?? 0, $"seed must be {SEED_BYTES} bytes in length.");

            var publicKey = new byte[PUBLIC_KEY_BYTES];
            var privateKey = new byte[SECRET_KEY_BYTES];

            Ed25519.GeneratePublicKey(seed, 0, publicKey, 0);

            Buffer.BlockCopy(seed, 0, privateKey, 0, SEED_BYTES);
            Buffer.BlockCopy(publicKey, 0, privateKey, SEED_BYTES, PUBLIC_KEY_BYTES);

            return new KeyPair(publicKey, privateKey);
        }

        /// <summary>Signs a message with Ed25519.</summary>
        /// <param name="message">The message.</param>
        /// <param name="key">The 64 byte private key.</param>
        /// <returns>The signature.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static byte[] SignDetached(byte[] message, byte[] key)
        {
            if (key == null || key.Length != SECRET_KEY_BYTES)
                throw new ArgumentOutOfRangeException(nameof(key), key?.Length ?? 0, $"key must be {SECRET_KEY_BYTES} bytes in length.");

            var signature = new byte[BYTES];

            Ed25519.Sign(key, 0, message, 0, message.Length, signature, 0);

            return signature;
        }

        /// <summary>Converts the ed25519 public key to curve25519 public key.</summary>
        /// <param name="ed25519PublicKey">Ed25519 public key.</param>
        /// <returns>The curve25519 public key.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static byte[] ConvertEd25519PublicKeyToCurve25519PublicKey(byte[] ed25519PublicKey)
        {
            return Ed25519Extensions.ConvertEd25519PublicKeyToCurve25519PublicKey(ed25519PublicKey);
        }

        /// <summary>Converts the ed25519 secret key to curve25519 secret key.</summary>
        /// <param name="ed25519SecretKey">Ed25519 secret key.</param>
        /// <returns>The curve25519 secret key.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static byte[] ConvertEd25519SecretKeyToCurve25519SecretKey(byte[] ed25519SecretKey)
        {
            return Ed25519Extensions.ConvertEd25519SecretKeyToCurve25519SecretKey(ed25519SecretKey);
        }
    }
}