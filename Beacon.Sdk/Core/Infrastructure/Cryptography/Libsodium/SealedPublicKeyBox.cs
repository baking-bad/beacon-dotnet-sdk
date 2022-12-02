using System;
using System.Text;

using Beacon.Sdk.Core.Infrastructure.Cryptography.Libsodium;

namespace Beacon.Sdk.Core.Infrastructure.Cryptography
{
    /// <summary> Create and Open SealedPublicKeyBoxes. </summary>
    public static class SealedPublicKeyBox
    {
        public const int RecipientPublicKeyBytes = Sodium.crypto_box_curve25519xsalsa20poly1305_PUBLICKEYBYTES;
        public const int RecipientSecretKeyBytes = Sodium.crypto_box_curve25519xsalsa20poly1305_SECRETKEYBYTES;
        private const int CryptoBoxSealbytes = Sodium.crypto_box_curve25519xsalsa20poly1305_PUBLICKEYBYTES + Sodium.crypto_box_curve25519xsalsa20poly1305_MACBYTES;

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

            var buffer = new byte[message.Length + CryptoBoxSealbytes];

            Sodium.Initialize();
            var ret = Sodium.CryptoBoxSeal(buffer, message, (ulong)message.Length, recipientPublicKey);

            if (ret != 0)
                throw new Exception("Failed to create SealedBox");

            return buffer;
        }

        /// <summary>Opens a SealedPublicKeyBox</summary>
        /// <param name="cipherText">The cipherText to be opened.</param>
        /// <param name="recipientSecretKey">The recipient's secret key.</param>
        /// <param name="recipientPublicKey">The recipient's public key.</param>
        /// <returns>The decrypted message.</returns>
        /// <exception cref="KeyOutOfRangeException"></exception>
        /// <exception cref="CryptographicException"></exception>
        public static byte[] Open(byte[] cipherText, byte[] recipientSecretKey, byte[] recipientPublicKey)
        {
            if (recipientSecretKey == null || recipientSecretKey.Length != RecipientSecretKeyBytes)
                throw new ArgumentOutOfRangeException(nameof(recipientPublicKey), recipientSecretKey?.Length ?? 0, $"recipientSecretKey must be {RecipientSecretKeyBytes} bytes in length.");
            if (recipientPublicKey == null || recipientPublicKey.Length != RecipientPublicKeyBytes)
                throw new ArgumentOutOfRangeException(nameof(recipientPublicKey), recipientPublicKey?.Length ?? 0, $"recipientPublicKey must be {RecipientPublicKeyBytes} bytes in length.");

            if (cipherText.Length < CryptoBoxSealbytes)
                throw new Exception("Failed to open SealedBox");

            var buffer = new byte[cipherText.Length - CryptoBoxSealbytes];

            Sodium.Initialize();
            var ret = Sodium.CryptoBoxSealOpen(buffer, cipherText, (ulong)cipherText.Length, recipientPublicKey,
                recipientSecretKey);

            if (ret != 0)
                throw new Exception("Failed to open SealedBox");

            return buffer;
        }
    }
}