namespace Beacon.Sdk.Core.Domain.Interfaces.Data
{
    using Matrix.Sdk.Core.Utils;

    public interface IBeaconPeerRepository
    {
        BeaconPeer Create(string name, HexString hexPublicKey, string version);
        
        BeaconPeer? TryReadByUserId(string userId);
    }
}