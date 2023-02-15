using System;
using System.Text;

using Org.BouncyCastle.Crypto.Digests;

namespace Beacon.Sdk.Core.Infrastructure.Cryptography
{
    public static partial class GenericHash
    {
        private const int BYTES_MIN = 16;
        private const int BYTES_MAX = 64;

        /// <summary>Hashes a message, using the BLAKE2b primitive.</summary>
        /// <param name="message">The message to be hashed.</param>
        /// <param name="resultLength">The size (in bytes) of the desired result.</param>
        /// <returns>Returns a byte array.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static byte[] Hash(string message, int resultLength)
        {
            return Hash(Encoding.UTF8.GetBytes(message), resultLength);
        }

        /// <summary>Hashes a message, using the BLAKE2b primitive.</summary>
        /// <param name="message">The message to be hashed.</param>
        /// <param name="resultLength">The size (in bytes) of the desired result.</param>
        /// <returns>Returns a byte array.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static byte[] Hash(byte[] message, int resultLength)
        {
            if (resultLength > BYTES_MAX || resultLength < BYTES_MIN)
                throw new ArgumentOutOfRangeException(nameof(resultLength), resultLength, $"bytes must be between {BYTES_MIN} and {BYTES_MAX} bytes in length.");

            var array = new byte[resultLength];

            var blake2bDigest = new Blake2bDigest(resultLength * 8);
            blake2bDigest.BlockUpdate(message, 0, message.Length);
            blake2bDigest.DoFinal(array, 0);

            return array;
        }
    }
}