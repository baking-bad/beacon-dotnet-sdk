namespace Beacon.Sdk.Core.Infrastructure.Repositories
{
    using System;
    using System.Collections.Generic;
    using Cryptography.Libsodium;
    using Domain.Interfaces.Data;
    using Matrix.Sdk.Core.Utils;
    using Sodium;

    public class InMemoryKeyPairRepository : IKeyPairRepository
    {
        private static readonly Dictionary<HexString, KeyPair> ClientSessionKeyPairs = new();
        private static readonly Dictionary<HexString, SessionKeyPair> ServerSessionKeyPairs = new();
        private readonly ICryptographyService _cryptographyService;

        public InMemoryKeyPairRepository(ICryptographyService cryptographyService)
        {
            _cryptographyService = cryptographyService;
        }

        public SessionKeyPair CreateOrReadServerSession(HexString clientPublicKey, KeyPair serverKeyPair) =>
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
        
        private KeyPair? _keyPair;
        public KeyPair KeyPair
        {
            get
            {
                // Todo: add storage
                if (_keyPair != null)
                    return _keyPair;

                var seed = Guid.NewGuid().ToString();
                _keyPair = _cryptographyService.GenerateEd25519KeyPair(seed);

                return _keyPair;   
            }
        }
    }
}