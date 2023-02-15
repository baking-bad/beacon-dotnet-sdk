using Beacon.Sdk.Core.Infrastructure.Cryptography;

namespace Beacon.Sdk.Core.Infrastructure.Repositories
{
    using System.Collections.Concurrent;
    using Domain.Interfaces;
    using Domain.Interfaces.Data;
    using Utils;

    public class InMemorySessionKeyPairRepository : ISessionKeyPairRepository
    {
        private readonly ConcurrentDictionary<HexString, SessionKeyPair> _clientSessionKeyPairs;
        private readonly ConcurrentDictionary<HexString, SessionKeyPair> _serverSessionKeyPairs;
        private readonly ICryptographyService _cryptographyService;

        public InMemorySessionKeyPairRepository(ICryptographyService cryptographyService)
        {
            _cryptographyService = cryptographyService;
            _clientSessionKeyPairs = new();
            _serverSessionKeyPairs = new();
        }

        public SessionKeyPair CreateOrReadClient(HexString clientHexPublicKey, KeyPair serverKeyPair) =>
            _clientSessionKeyPairs.TryGetValue(clientHexPublicKey, out SessionKeyPair sessionKeyPair)
                ? sessionKeyPair
                : _clientSessionKeyPairs[clientHexPublicKey] =
                    _cryptographyService.CreateClientSessionKeyPair(clientHexPublicKey.ToByteArray(),
                        serverKeyPair.PrivateKey);

        // public SessionKeyPair CreateOrReadClient(HexString clientHexPublicKey, KeyPair serverKeyPair) =>
        //     _cryptographyService.CreateClientSessionKeyPair(clientHexPublicKey.ToByteArray(), serverKeyPair.PrivateKey);
        //
        // public SessionKeyPair CreateOrReadServer(HexString clientHexPublicKey, KeyPair serverKeyPair) =>
        //     _cryptographyService.CreateServerSessionKeyPair(clientHexPublicKey.ToByteArray(), serverKeyPair.PrivateKey);

        public SessionKeyPair CreateOrReadServer(HexString clientHexPublicKey, KeyPair serverKeyPair) =>
            _serverSessionKeyPairs.TryGetValue(clientHexPublicKey, out SessionKeyPair sessionKeyPair)
                ? sessionKeyPair
                : _serverSessionKeyPairs[clientHexPublicKey] = _cryptographyService
                    .CreateServerSessionKeyPair(clientHexPublicKey.ToByteArray(), serverKeyPair.PrivateKey);
    }
}