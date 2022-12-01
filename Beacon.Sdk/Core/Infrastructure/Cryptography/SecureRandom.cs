using System.Security.Cryptography;

namespace Beacon.Sdk.Core.Infrastructure.Cryptography
{
    public static class SecureRandom
    {
        public static byte[] GetRandomBytes(int count)
        {
            var rng = new RNGCryptoServiceProvider();

            var bytes = new byte[count];
            rng.GetBytes(bytes);

            return bytes;
        }
    }
}