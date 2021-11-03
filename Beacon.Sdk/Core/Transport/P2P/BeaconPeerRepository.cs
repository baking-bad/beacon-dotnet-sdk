namespace Beacon.Sdk.Core.Transport.P2P
{
    public interface IBeaconPeerRepository
    {
        BeaconPeer CreateOrUpdate(string key, BeaconPeer beaconPeer);
        
        BeaconPeer? TryRead(string key);
    }
}