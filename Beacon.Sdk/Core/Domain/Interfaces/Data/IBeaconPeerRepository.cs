namespace Beacon.Sdk.Core.Domain.Interfaces.Data
{
    using System.Threading.Tasks;

    public interface IBeaconPeerRepository
    {
        Task<BeaconPeer> Create(BeaconPeer peer);

        Task<BeaconPeer?> TryReadByUserId(string userId);
    }
}