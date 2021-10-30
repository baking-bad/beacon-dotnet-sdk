namespace Matrix.Sdk.Core.Domain.Services
{
    using Sodium;

    public interface ICryptographyService
    {
        string ToHexString(byte[] input);

        byte[] GenerateLoginDigest();

        KeyPair GenerateEd25519KeyPair(string seed);

        string GenerateHexSignature(byte[] loginDigest, byte[] secretKey);

        string GenerateHexId(byte[] publicKey);
    }
}