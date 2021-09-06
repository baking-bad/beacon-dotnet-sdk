namespace BeaconSdk.Repositories
{
    using System.Collections.Generic;
    using Infrastructure.Cryptography;
    using Infrastructure.Cryptography.Libsodium;
    using MatrixSdk.Utils;
    using Sodium;

    public class SessionKeyPairInMemory
    {
        private static readonly Dictionary<HexString, KeyPair> clientSessionKeyPairs = new();
        private static readonly Dictionary<HexString, SessionKeyPair> ServerSessionKeyPairs = new();

        public static SessionKeyPair CreateOrReadServer(HexString clientPublicKey, KeyPair serverKeyPair) =>
            ServerSessionKeyPairs.TryGetValue(clientPublicKey, out var sessionKeyPair)
                ? sessionKeyPair
                : CreateServerSessionKeyPair(clientPublicKey, serverKeyPair);
        
        private static SessionKeyPair CreateServerSessionKeyPair(HexString clientPublicKey, KeyPair serverKeyPair)
        {
            var sessionKeyPair = EncryptionService.CreateServerSessionKeyPair(clientPublicKey.ToByteArray(), serverKeyPair.PrivateKey);

            ServerSessionKeyPairs[clientPublicKey] = sessionKeyPair;

            return sessionKeyPair;
        }
    }
}