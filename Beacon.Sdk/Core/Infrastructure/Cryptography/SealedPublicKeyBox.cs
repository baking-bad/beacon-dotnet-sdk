using System;
using System.Text;

using NaCl;
using Org.BouncyCastle.Crypto.Digests;

namespace Beacon.Sdk.Core.Infrastructure.Cryptography
{
    /// <summary> Create and Open SealedPublicKeyBoxes. </summary>
    public static class SealedPublicKeyBox
    {
        public const int RecipientPublicKeyBytes = 32;
        public const int RecipientSecretKeyBytes = 32;
        private const int PublicKeyBytes = 32;
        private const int MacBytes = 16;
        private const int NonceBytes = 24;

        /// <summary> Creates a SealedPublicKeyBox</summary>
        /// <param name="message">The message.</param>
        /// <param name="recipientPublicKey">The 32 byte recipient's public key.</param>
        /// <returns>The anonymously encrypted message.</returns>
        /// <exception cref="KeyOutOfRangeException"></exception>
        /// <exception cref="CryptographicException"></exception>
        public static byte[] Create(string message, byte[] recipientPublicKey)
        {
            return Create(Encoding.UTF8.GetBytes(message), recipientPublicKey);
        }

        /// <summary> Creates a SealedPublicKeyBox</summary>
        /// <param name="message">The message.</param>
        /// <param name="recipientPublicKey">The 32 byte recipient's public key.</param>
        /// <returns>The anonymously encrypted message.</returns>
        /// <exception cref="KeyOutOfRangeException"></exception>
        /// <exception cref="CryptographicException"></exception>
        public static byte[] Create(byte[] message, byte[] recipientPublicKey)
        {
            if (recipientPublicKey == null || recipientPublicKey.Length != RecipientPublicKeyBytes)
                throw new ArgumentOutOfRangeException(nameof(recipientPublicKey), recipientPublicKey?.Length ?? 0, $"recipientPublicKey must be {RecipientPublicKeyBytes} bytes in length.");

            var buffer = new byte[message.Length + PublicKeyBytes + MacBytes];

            Curve25519XSalsa20Poly1305.KeyPair(out var esk, out var epk);

            var secretBoxSeal = new Curve25519XSalsa20Poly1305(esk, epk);

            var nonce = GetSealNonce(epk, recipientPublicKey);

            secretBoxSeal.Encrypt(
                cipher: new Span<byte>(buffer)[PublicKeyBytes..], // skip first 32 bytes for public key
                message: message,
                nonce: nonce);

            Buffer.BlockCopy(epk, 0, buffer, 0, PublicKeyBytes);

            //var ret = Sodium.CryptoBoxSeal(
            //    buffer,
            //    message,
            //    (ulong)message.Length,
            //    recipientPublicKey);

            //if (ret != 0)
            //    throw new Exception("Failed to create SealedBox");

            return buffer;
        }

        /// <summary>Opens a SealedPublicKeyBox</summary>
        /// <param name="cipherText">The cipherText to be opened.</param>
        /// <param name="recipientSecretKey">The recipient's secret key.</param>
        /// <param name="recipientPublicKey">The recipient's public key.</param>
        /// <returns>The decrypted message.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static byte[] Open(byte[] cipherText, byte[] recipientSecretKey, byte[] recipientPublicKey)
        {
            if (recipientSecretKey == null || recipientSecretKey.Length != RecipientSecretKeyBytes)
                throw new ArgumentOutOfRangeException(nameof(recipientPublicKey), recipientSecretKey?.Length ?? 0, $"recipientSecretKey must be {RecipientSecretKeyBytes} bytes in length.");
            if (recipientPublicKey == null || recipientPublicKey.Length != RecipientPublicKeyBytes)
                throw new ArgumentOutOfRangeException(nameof(recipientPublicKey), recipientPublicKey?.Length ?? 0, $"recipientPublicKey must be {RecipientPublicKeyBytes} bytes in length.");

            if (cipherText.Length < PublicKeyBytes + MacBytes)
                throw new Exception("Failed to open SealedBox");

            var buffer = new byte[cipherText.Length - (PublicKeyBytes + MacBytes)];

            var nonce = GetSealNonce(cipherText, recipientPublicKey);

            var secretBoxSeal = new Curve25519XSalsa20Poly1305(recipientSecretKey, recipientPublicKey);

            if (!secretBoxSeal.TryDecrypt(
                message: buffer,
                cipher: new ReadOnlySpan<byte>(cipherText)[PublicKeyBytes..],
                nonce: nonce))
            {
                throw new Exception("Failed to open SealedBox");
            }

            //var ret = Sodium.CryptoBoxSealOpen(
            //    buffer,
            //    cipherText,
            //    (ulong)cipherText.Length,
            //    recipientPublicKey,
            //    recipientSecretKey);

            //if (ret != 0)
            //    throw new Exception("Failed to open SealedBox");

            return buffer;
        }

        private static byte[] GetSealNonce(byte[] pk1, byte[] pk2)
        {
            var nonce = new byte[NonceBytes];

            var blake2bDigest = new Blake2bDigest(NonceBytes * 8);
            blake2bDigest.BlockUpdate(pk1, 0, PublicKeyBytes);
            blake2bDigest.BlockUpdate(pk2, 0, PublicKeyBytes);
            blake2bDigest.DoFinal(nonce, 0);

            return nonce;
        }
    }
}