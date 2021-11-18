namespace Beacon.Sdk.Core.Domain.Interfaces.Data
{
    using Utils;

    public interface IBeaconPeerRepository
    {
        BeaconPeer Create(string name, HexString hexPublicKey, string version);
        
        BeaconPeer? TryReadByUserId(string userId);
    }
}