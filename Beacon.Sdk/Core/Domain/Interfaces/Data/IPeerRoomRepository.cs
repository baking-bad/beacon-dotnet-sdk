namespace Beacon.Sdk.Core.Domain.Interfaces.Data
{
    using System.Threading.Tasks;
    using Utils;

    public interface IPeerRoomRepository
    {
        Task<PeerRoom?> TryRead(HexString peerPublicKey);

        Task<PeerRoom> CreateOrUpdate(PeerRoom peerRoom);
    }
}