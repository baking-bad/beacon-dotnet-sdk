using System.Collections.Generic;

namespace Beacon.Sdk.Core.Domain.Interfaces.Data
{
    using System.Threading.Tasks;
    using Entities.P2P;
    using Utils;

    public interface IP2PPeerRoomRepository
    {
        Task<P2PPeerRoom> CreateOrUpdateAsync(P2PPeerRoom p2PPeerRoom);

        Task<P2PPeerRoom?> TryReadAsync(string p2PUserId);

        Task<P2PPeerRoom?> TryReadAsync(HexString peerHexPublicKey);

        Task<List<P2PPeerRoom>> GetAll();

        Task Remove(HexString peerHexPublicKey);
    }
}