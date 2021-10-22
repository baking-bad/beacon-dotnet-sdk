namespace Beacon.Sdk.Core.Infrastructure.Repositories
{
    using System.Collections.Generic;
    using Cryptography;
    using Cryptography.Libsodium;
    using Matrix.Sdk.Core.Utils;
    using Sodium;

    public class SessionKeyPairInMemory
    {
        private static readonly Dictionary<HexString, KeyPair> ClientSessionKeyPairs = new();
        private static readonly Dictionary<HexString, SessionKeyPair> ServerSessionKeyPairs = new();

        public static SessionKeyPair CreateOrReadServer(HexString clientPublicKey, KeyPair serverKeyPair) =>
            ServerSessionKeyPairs.TryGetValue(clientPublicKey, out SessionKeyPair sessionKeyPair)
                ? sessionKeyPair
                : CreateServerSessionKeyPair(clientPublicKey, serverKeyPair);

        private static SessionKeyPair CreateServerSessionKeyPair(HexString clientPublicKey, KeyPair serverKeyPair)
        {
            SessionKeyPair sessionKeyPair =
                BeaconCryptographyService.CreateServerSessionKeyPair(clientPublicKey.ToByteArray(),
                    serverKeyPair.PrivateKey);

            ServerSessionKeyPairs[clientPublicKey] = sessionKeyPair;

            return sessionKeyPair;
        }
    }
}