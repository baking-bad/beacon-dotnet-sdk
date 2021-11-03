namespace Beacon.Sdk.Core.Infrastructure.Repositories
{
    using System.Collections.Concurrent;
    using Transport.P2P;

    public class BeaconPeerRepositoryInMemory : IBeaconPeerRepository
    {
        private static readonly ConcurrentDictionary<string, BeaconPeer> InMemoryBeaconPeers = new();
       
        public BeaconPeer CreateOrUpdate(string key, BeaconPeer beaconPeer)
        {
            InMemoryBeaconPeers[key] = beaconPeer;

            return InMemoryBeaconPeers[key];
        }
        
        public BeaconPeer? TryRead(string key) => InMemoryBeaconPeers.TryGetValue(key, out BeaconPeer? beaconPeer) 
            ? beaconPeer 
            : null;
    }
}