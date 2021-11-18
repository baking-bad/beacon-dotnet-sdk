namespace Beacon.Sdk.Core.Infrastructure.Repositories
{
    using System.Collections.Concurrent;
    using Domain;
    using Domain.Interfaces.Data;
    using Utils;

    public class InMemoryBeaconPeerRepository : IBeaconPeerRepository
    {
        private readonly ICryptographyService _cryptographyService;
        private static readonly ConcurrentDictionary<string, BeaconPeer> InMemoryBeaconPeers = new();
        
        public InMemoryBeaconPeerRepository(ICryptographyService cryptographyService)
        {
            _cryptographyService = cryptographyService;
        }

        public BeaconPeer Create(string name, HexString hexPublicKey, string version)
        {
            BeaconPeer beaconPeer = BeaconPeer.Factory.Create(_cryptographyService, name, hexPublicKey, version);
            InMemoryBeaconPeers[beaconPeer.UserId] = beaconPeer;

            return InMemoryBeaconPeers[beaconPeer.UserId];
        }

        public BeaconPeer? TryReadByUserId(string userId) => InMemoryBeaconPeers.TryGetValue(userId, out BeaconPeer? beaconPeer)
            ? beaconPeer
            : null;
    }
}