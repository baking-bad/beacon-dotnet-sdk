namespace MatrixSdk.Infrastructure.Services
{
    using System;
    using Sodium;

    public class SignatureCryptoService
    {
        public static string ToHexString(byte[] input)
        {
            var hexString = BitConverter.ToString(input);

            var result = hexString.Replace("-", "");

            return result.ToLower();
        }

        public static byte[] GenerateLoginDigest()
        {
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds() * 1000;
            var message = $"login:{now / 1000 / (5 * 60)}";

            return GenericHash.Hash(message, (byte[]?)null, 32);
        }

        public static KeyPair GenerateEd25519KeyPair(string seed)
        {
            var hash = GenericHash.Hash(seed, (byte[]?)null, 32);

            return PublicKeyAuth.GenerateKeyPair(hash);
        }

        public static string GenerateHexSignature(byte[] loginDigest, byte[] secretKey)
        {
            var signature = PublicKeyAuth.SignDetached(loginDigest, secretKey);

            return ToHexString(signature);
        }

        public static string GenerateHexId(byte[] publicKey)
        {
            var hash = GenericHash.Hash(publicKey, null, publicKey.Length);

            return ToHexString(hash);
        }
    }
}

// Todo: maybe use https://nsec.rocks/ ?
// public class NSecCryptoService
// {
//     public void GenerateKeyPairFromSeed(string seed)
//     {
//         var algorithm = SignatureAlgorithm.Ed25519;
//         using var key = Key.Create(algorithm);
//         // key.PublicKey;
//         // algorithm.si
//         // key.PublicKey
//         // algorithm.Sign(key, seed);
//
//         // t.PrivateKeySize
//         // var t = new SignatureAlgorithm.Ed25519()
//     }
// }