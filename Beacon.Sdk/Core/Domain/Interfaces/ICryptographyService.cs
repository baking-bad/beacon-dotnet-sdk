using Beacon.Sdk.Core.Infrastructure.Cryptography;

namespace Beacon.Sdk.Core.Domain.Interfaces
{
    using Utils;

    public interface ICryptographyService
    {
        SessionKeyPair CreateClientSessionKeyPair(byte[] clientPublicKey, byte[] serverPrivateKey);

        SessionKeyPair CreateServerSessionKeyPair(byte[] clientPublicKey, byte[] serverPrivateKey);

        byte[] Hash(byte[] input);

        byte[] Hash(byte[] message, int bufferLength);

        KeyPair GenerateEd25519KeyPair(string seed);

        HexString Encrypt(string input, byte[] key);

        string Decrypt(HexString hexInput, byte[] key);

        string ToHexString(byte[] input);

        byte[] ConvertEd25519PublicKeyToCurve25519PublicKey(byte[] publicKey);

        byte[] ConvertEd25519SecretKeyToCurve25519SecretKey(byte[] secretKey);

        string EncryptMessageAsString(string message, byte[] publicKey);

        string DecryptMessageAsString(string message, HexString secretKey, HexString publicKey);

        bool Validate(string input);

        byte[] GenerateLoginDigest();

        string GenerateHexSignature(byte[] loginDigest, byte[] secretKey);

        string GenerateHexId(byte[] publicKey);
    }
}