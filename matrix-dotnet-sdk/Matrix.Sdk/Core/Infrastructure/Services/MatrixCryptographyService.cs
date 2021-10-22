namespace Matrix.Sdk.Core.Infrastructure.Services
{
    using System;
    using Sodium;

    public class MatrixCryptographyService
    {
        public static string ToHexString(byte[] input)
        {
            var hexString = BitConverter.ToString(input);

            string result = hexString.Replace("-", "");

            return result.ToLower();
        }

        public static byte[] GenerateLoginDigest()
        {
            long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds() * 1000;
            var message = $"login:{now / 1000 / (5 * 60)}";

            return GenericHash.Hash(message, (byte[]?) null, 32);
        }

        public static KeyPair GenerateEd25519KeyPair(string seed)
        {
            byte[] hash = GenericHash.Hash(seed, (byte[]?) null, 32);

            return PublicKeyAuth.GenerateKeyPair(hash);
        }

        public static string GenerateHexSignature(byte[] loginDigest, byte[] secretKey)
        {
            byte[] signature = PublicKeyAuth.SignDetached(loginDigest, secretKey);

            return ToHexString(signature);
        }

        public static string GenerateHexId(byte[] publicKey)
        {
            byte[] hash = GenericHash.Hash(publicKey, null, publicKey.Length);

            return ToHexString(hash);
        }
    }
}