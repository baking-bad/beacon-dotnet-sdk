namespace Beacon.Sdk.Core.Domain.Interfaces.Data
{
    using Utils;

    public interface IBeaconPeerRepository
    {
        BeaconPeer Create(string name, string relaySever, HexString hexPublicKey, string version);
        
        BeaconPeer? TryReadByUserId(string userId);
    }
}