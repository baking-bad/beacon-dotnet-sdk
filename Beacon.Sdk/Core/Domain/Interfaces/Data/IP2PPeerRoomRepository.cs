namespace Beacon.Sdk.Core.Domain.Interfaces.Data
{
    using System.Threading.Tasks;
    using P2P;
    using Utils;

    public interface IP2PPeerRoomRepository
    {
        Task<P2PPeerRoom?> TryReadByP2PUserId(string p2PUserId);

        Task<P2PPeerRoom?> TryReadByPeerHexPublicKey(HexString peerHexPublicKey);

        Task<P2PPeerRoom> CreateOrUpdate(P2PPeerRoom p2PPeerRoom);
    }
}