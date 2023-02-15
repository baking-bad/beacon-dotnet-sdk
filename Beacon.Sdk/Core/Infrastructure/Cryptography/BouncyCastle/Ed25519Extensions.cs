using Org.BouncyCastle.Crypto.Digests;
using System;

namespace Beacon.Sdk.Core.Infrastructure.Cryptography.BouncyCastle
{
    using NaCl;

    internal static class Ed25519Extensions
    {
        private const int PUBLIC_KEY_BYTES = 32;
        private const int SCALAR_BYTES = 32;
        private const int SECRET_KEY_BYTES = 32 + 32;

        /// <summary>Converts the ed25519 public key to curve25519 public key.</summary>
        /// <param name="ed25519PublicKey">Ed25519 public key.</param>
        /// <returns>The curve25519 public key.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static byte[] ConvertEd25519PublicKeyToCurve25519PublicKey(byte[] ed25519PublicKey)
        {
            if (ed25519PublicKey == null || ed25519PublicKey.Length != PUBLIC_KEY_BYTES)
                throw new ArgumentOutOfRangeException(nameof(ed25519PublicKey), ed25519PublicKey?.Length ?? 0, $"ed25519PublicKey must be {PUBLIC_KEY_BYTES} bytes in length.");
            
            var result = new byte[32];
            MontgomeryCurve25519.EdwardsToMontgomery(result, ed25519PublicKey);
            return result;
        }

        /// <summary>Converts the ed25519 secret key to curve25519 secret key.</summary>
        /// <param name="ed25519SecretKey">Ed25519 secret key.</param>
        /// <returns>The curve25519 secret key.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static byte[] ConvertEd25519SecretKeyToCurve25519SecretKey(byte[] ed25519SecretKey)
        {
            // key can be appended with the public key or not (both are allowed)
            if (ed25519SecretKey == null || (ed25519SecretKey.Length != PUBLIC_KEY_BYTES && ed25519SecretKey.Length != SECRET_KEY_BYTES))
                throw new ArgumentOutOfRangeException(nameof(ed25519SecretKey), ed25519SecretKey?.Length ?? 0, $"ed25519SecretKey must be either {PUBLIC_KEY_BYTES} or {SECRET_KEY_BYTES} bytes in length.");

            var sha512 = new Sha512Digest();

            byte[] h = new byte[sha512.GetDigestSize()];

            sha512.BlockUpdate(ed25519SecretKey, 0, SCALAR_BYTES);
            sha512.DoFinal(h, 0);

            PruneScalar(h);

            return h.AsSpan(0, 32).ToArray();
        }

        private static void PruneScalar(byte[] n)
        {
            n[0] &= 0xF8;
            n[SCALAR_BYTES - 1] &= 0x7F;
            n[SCALAR_BYTES - 1] |= 0x40;
        }
    }
}