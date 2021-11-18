namespace Beacon.Sdk.Core.Domain.Interfaces.Data
{
    using Infrastructure.Cryptography.Libsodium;
    using Sodium;
    using Utils;

    public interface ICryptographyService
    {
        SessionKeyPair CreateServerSessionKeyPair(byte[] clientPublicKey, byte[] serverSecretKey);
        
        byte[] Hash(byte[] input);
        
        byte[] Decrypt(byte[] encryptedBytes, byte[] sharedKey);
        
        byte[] Encrypt(byte[] message, byte[] recipientPublicKey);
        
        KeyPair GenerateEd25519KeyPair(string seed);
        
        HexString EncryptAsHex(string message, byte[] recipientPublicKey);
        
        string DecryptAsString(string encryptedMessage, byte[] sharedKey);
        
        string ToHexString(byte[] input);
        
        byte[] ConvertEd25519PublicKeyToCurve25519PublicKey(byte[] publicKey);
        
        string EncryptMessageAsString(string message, byte[] publicKey);
        
        bool Validate(string input);
        
        byte[] GenerateLoginDigest();
        
        string GenerateHexSignature(byte[] loginDigest, byte[] secretKey);
        
        string GenerateHexId(byte[] publicKey);
    }
}