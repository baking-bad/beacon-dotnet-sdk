using System;
using System.Text;

using Beacon.Sdk.Core.Infrastructure.Cryptography.Libsodium;

namespace Beacon.Sdk.Core.Infrastructure.Cryptography
{
    public static partial class GenericHash
    {
        private const int BYTES_MIN = SodiumLibrary.crypto_generichash_blake2b_BYTES_MIN;
        private const int BYTES_MAX = SodiumLibrary.crypto_generichash_blake2b_BYTES_MAX;
        private const int KEY_BYTES_MIN = SodiumLibrary.crypto_generichash_blake2b_KEYBYTES_MIN;
        private const int KEY_BYTES_MAX = SodiumLibrary.crypto_generichash_blake2b_KEYBYTES_MAX;

        /// <summary>Generates a random 64 byte key.</summary>
        /// <returns>Returns a byte array with 64 random bytes</returns>
        public static byte[] GenerateKey()
        {
            return SecureRandom.GetRandomBytes(KEY_BYTES_MAX);
        }

        /// <summary>Hashes a message, with an optional key, using the BLAKE2b primitive.</summary>
        /// <param name="message">The message to be hashed.</param>
        /// <param name="key">The key; may be null, otherwise between 16 and 64 bytes.</param>
        /// <param name="bytes">The size (in bytes) of the desired result.</param>
        /// <returns>Returns a byte array.</returns>
        /// <exception cref="KeyOutOfRangeException"></exception>
        /// <exception cref="BytesOutOfRangeException"></exception>
        public static byte[] Hash(string message, string? key, int bytes)
        {
            return Hash(message, key != null ? Encoding.UTF8.GetBytes(key) : null, bytes);
        }

        /// <summary>Hashes a message, with an optional key, using the BLAKE2b primitive.</summary>
        /// <param name="message">The message to be hashed.</param>
        /// <param name="key">The key; may be null, otherwise between 16 and 64 bytes.</param>
        /// <param name="bytes">The size (in bytes) of the desired result.</param>
        /// <returns>Returns a byte array.</returns>
        /// <exception cref="KeyOutOfRangeException"></exception>
        /// <exception cref="BytesOutOfRangeException"></exception>
        public static byte[] Hash(string message, byte[]? key, int bytes)
        {
            return Hash(Encoding.UTF8.GetBytes(message), key, bytes);
        }

        /// <summary>Hashes a message, with an optional key, using the BLAKE2b primitive.</summary>
        /// <param name="message">The message to be hashed.</param>
        /// <param name="key">The key; may be null, otherwise between 16 and 64 bytes.</param>
        /// <param name="bytes">The size (in bytes) of the desired result.</param>
        /// <returns>Returns a byte array.</returns>
        /// <exception cref="KeyOutOfRangeException"></exception>
        /// <exception cref="BytesOutOfRangeException"></exception>
        public static byte[] Hash(byte[] message, byte[]? key, int bytes)
        {
            if (key == null)
                key = Array.Empty<byte>();
            else if (key.Length > KEY_BYTES_MAX || key.Length < KEY_BYTES_MIN)
                throw new ArgumentOutOfRangeException(nameof(key), key?.Length ?? 0, $"key must be between {KEY_BYTES_MIN} and {KEY_BYTES_MAX} bytes in length.");
            if (bytes > BYTES_MAX || bytes < BYTES_MIN)
                throw new ArgumentOutOfRangeException(nameof(bytes), bytes, $"bytes must be between {BYTES_MIN} and {BYTES_MAX} bytes in length.");

            var buffer = new byte[bytes];

            SodiumCore.Initialize();
            SodiumLibrary.crypto_generichash_blake2b(buffer, (nuint)buffer.Length, message, (nuint)message.Length, key, (nuint)key.Length);

            return buffer;
        }
    }
}