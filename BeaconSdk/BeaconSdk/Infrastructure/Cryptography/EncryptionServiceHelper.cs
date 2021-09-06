namespace BeaconSdk.Infrastructure.Cryptography
{
    using System;
    using System.Text;
    using MatrixSdk.Utils;

    public static class EncryptionServiceHelper
    {
        public static HexString EncryptAsHex(string message, byte[] sharedKey)
        {
            var bytes = HexString.TryParse(message, out var hexString)
                ? hexString.ToByteArray()
                : Encoding.UTF8.GetBytes(message);

            var encryptedBytes = EncryptionService.Encrypt(bytes, sharedKey);

            if (!HexString.TryParse(encryptedBytes, out var result))
                throw new InvalidOperationException("Can not parse encryptedBytes");
            
            return result;
        }
        
        public static string DecryptAsString(string encryptedMessage, byte[] sharedKey)
        {
            var encryptedBytes = HexString.TryParse(encryptedMessage, out var hexString)
                ? hexString.ToByteArray()
                : Encoding.UTF8.GetBytes(encryptedMessage);

            var decryptedBytes = EncryptionService.Decrypt(encryptedBytes, sharedKey);

            return Encoding.UTF8.GetString(decryptedBytes);
        }
    }
}