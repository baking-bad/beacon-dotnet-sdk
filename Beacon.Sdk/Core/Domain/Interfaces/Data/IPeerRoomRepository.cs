namespace Beacon.Sdk.Core.Domain.Interfaces.Data
{
    using System.Threading.Tasks;
    using Utils;

    public interface IPeerRoomRepository
    {
        Task<BeaconPeerRoom?> TryRead(HexString peerPublicKey);

        Task<BeaconPeerRoom> CreateOrUpdate(BeaconPeerRoom beaconPeerRoom);
    }
}