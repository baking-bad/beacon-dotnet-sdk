namespace Beacon.Sdk.Core.Infrastructure.Repositories
{
    using System.Threading.Tasks;
    using Domain.Interfaces.Data;
    using Domain.P2P;
    using Microsoft.Extensions.Logging;
    using Utils;

    public class LiteDbP2PPeerRoomRepository : BaseLiteDbRepository<P2PPeerRoom>, IP2PPeerRoomRepository
    {
        public LiteDbP2PPeerRoomRepository(ILogger<LiteDbP2PPeerRoomRepository> logger, RepositorySettings settings) :
            base(logger, settings)
        {
        }

        public Task<P2PPeerRoom> CreateOrUpdate(P2PPeerRoom p2PPeerRoom) =>
            InConnection(col =>
            {
                P2PPeerRoom? result = col.Query()
                    .Where(x => x.PeerHexPublicKey.Value == p2PPeerRoom.PeerHexPublicKey.Value)
                    .FirstOrDefault();

                if (result == null)
                {
                    col.Insert(p2PPeerRoom);
                    col.EnsureIndex(x => x.P2PUserId);
                    col.EnsureIndex(x => x.PeerHexPublicKey);
                }
                else
                {
                    col.Update(p2PPeerRoom);
                }

                return Task.FromResult(p2PPeerRoom);
            });

        public Task<P2PPeerRoom?> TryRead(string p2PUserId) =>
            InConnectionNullable(col =>
            {
                P2PPeerRoom? peerRoom = col.Query().Where(x => x.P2PUserId == p2PUserId)
                    .FirstOrDefault();

                return Task.FromResult(peerRoom ?? null);
            });

        public Task<P2PPeerRoom?> TryRead(HexString peerHexPublicKey) =>
            InConnectionNullable(col =>
            {
                P2PPeerRoom? peerRoom = col.Query().Where(x => x.PeerHexPublicKey.Value == peerHexPublicKey.Value)
                    .FirstOrDefault();

                return Task.FromResult(peerRoom ?? null);
            });
    }
}