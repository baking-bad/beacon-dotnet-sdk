namespace Beacon.Sdk.Core.Infrastructure.Repositories
{
    using System.Collections.Concurrent;
    using Cryptography.Libsodium;
    using Domain.Interfaces;
    using Domain.Interfaces.Data;
    using Sodium;
    using Utils;

    public class InMemorySessionKeyPairRepository : ISessionKeyPairRepository
    {
        private static readonly ConcurrentDictionary<HexString, SessionKeyPair> ClientSessionKeyPairs = new();
        private static readonly ConcurrentDictionary<HexString, SessionKeyPair> ServerSessionKeyPairs = new();

        private readonly ICryptographyService _cryptographyService;

        public InMemorySessionKeyPairRepository(ICryptographyService cryptographyService)
        {
            _cryptographyService = cryptographyService;
        }

        // public SessionKeyPair CreateOrReadClient(HexString clientHexPublicKey, KeyPair serverKeyPair) =>
        //     ClientSessionKeyPairs.TryGetValue(clientHexPublicKey, out SessionKeyPair sessionKeyPair)
        //         ? sessionKeyPair
        //         : ClientSessionKeyPairs[clientHexPublicKey] =
        //             _cryptographyService.CreateClientSessionKeyPair(clientHexPublicKey.ToByteArray(),
        //                 serverKeyPair.PrivateKey);

        public SessionKeyPair CreateOrReadClient(HexString clientHexPublicKey, KeyPair serverKeyPair) =>
            _cryptographyService.CreateClientSessionKeyPair(clientHexPublicKey.ToByteArray(), serverKeyPair.PrivateKey);
        
        public SessionKeyPair CreateOrReadServer(HexString clientHexPublicKey, KeyPair serverKeyPair) =>
            _cryptographyService.CreateServerSessionKeyPair(clientHexPublicKey.ToByteArray(), serverKeyPair.PrivateKey);
        
        // public SessionKeyPair CreateOrReadServer(HexString clientHexPublicKey, KeyPair serverKeyPair) =>
        //     ServerSessionKeyPairs.TryGetValue(clientHexPublicKey, out SessionKeyPair sessionKeyPair)
        //         ? sessionKeyPair
        //         : ServerSessionKeyPairs[clientHexPublicKey] = _cryptographyService
        //             .CreateServerSessionKeyPair(clientHexPublicKey.ToByteArray(), serverKeyPair.PrivateKey);
    }
}