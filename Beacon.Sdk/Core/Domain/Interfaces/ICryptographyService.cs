namespace Beacon.Sdk.Core.Domain.Interfaces
{
    using Infrastructure.Cryptography.Libsodium;
    using Sodium;
    using Utils;

    public interface ICryptographyService
    {
        SessionKeyPair CreateClientSessionKeyPair(byte[] clientPublicKey, byte[] serverPrivateKey);

        SessionKeyPair CreateServerSessionKeyPair(byte[] clientPublicKey, byte[] serverPrivateKey);

        byte[] Hash(byte[] input);

        KeyPair GenerateEd25519KeyPair(string seed);

        HexString Encrypt(string input, byte[] key);

        string Decrypt(HexString hexInput, byte[] key);

        string ToHexString(byte[] input);

        byte[] ConvertEd25519PublicKeyToCurve25519PublicKey(byte[] publicKey);

        string EncryptMessageAsString(string message, byte[] publicKey);

        bool Validate(string input);

        byte[] GenerateLoginDigest();

        string GenerateHexSignature(byte[] loginDigest, byte[] secretKey);

        string GenerateHexId(byte[] publicKey);
    }
}