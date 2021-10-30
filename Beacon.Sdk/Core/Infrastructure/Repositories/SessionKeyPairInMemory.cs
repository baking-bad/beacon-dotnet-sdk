namespace Beacon.Sdk.Core.Infrastructure.Repositories
{
    using System.Collections.Generic;
    using Cryptography.Libsodium;
    using Matrix.Sdk.Core.Utils;
    using Sodium;

    public interface ISessionKeyPairRepository
    {
        SessionKeyPair CreateOrReadServer(HexString clientPublicKey, KeyPair serverKeyPair);
    }

    public class SessionKeyPairInMemoryRepository : ISessionKeyPairRepository
    {
        private static readonly Dictionary<HexString, KeyPair> ClientSessionKeyPairs = new();
        private static readonly Dictionary<HexString, SessionKeyPair> ServerSessionKeyPairs = new();
        private readonly ICryptographyService _cryptographyService;

        public SessionKeyPairInMemoryRepository(ICryptographyService cryptographyService)
        {
            _cryptographyService = cryptographyService;
        }

        public SessionKeyPair CreateOrReadServer(HexString clientPublicKey, KeyPair serverKeyPair) =>
            ServerSessionKeyPairs.TryGetValue(clientPublicKey, out SessionKeyPair sessionKeyPair)
                ? sessionKeyPair
                : CreateServerSessionKeyPair(clientPublicKey, serverKeyPair);

        private SessionKeyPair CreateServerSessionKeyPair(HexString clientPublicKey, KeyPair serverKeyPair)
        {
            SessionKeyPair sessionKeyPair =
                _cryptographyService.CreateServerSessionKeyPair(clientPublicKey.ToByteArray(),
                    serverKeyPair.PrivateKey);

            ServerSessionKeyPairs[clientPublicKey] = sessionKeyPair;

            return sessionKeyPair;
        }
    }
}