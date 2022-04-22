namespace Beacon.Sdk.Core.Infrastructure.Repositories
{
    using System.Threading.Tasks;
    using Domain.Entities.P2P;
    using Domain.Interfaces.Data;
    using Microsoft.Extensions.Logging;
    using Utils;

    public class LiteDbP2PPeerRoomRepository : BaseLiteDbRepository<P2PPeerRoom>, IP2PPeerRoomRepository
    {
        private const string CollectionName = "P2PPeerRoom";
        
        public LiteDbP2PPeerRoomRepository(ILogger<LiteDbP2PPeerRoomRepository> logger, RepositorySettings settings) :
            base(logger, settings)
        {
        }

        public Task<P2PPeerRoom> CreateOrUpdate(P2PPeerRoom p2PPeerRoom) =>
            InConnection(CollectionName,col =>
            {
                // P2PPeerRoom? result = col.Query()
                //     .Where(x => x.PeerHexPublicKey.Value == p2PPeerRoom.PeerHexPublicKey.Value)
                //     .FirstOrDefault();

                P2PPeerRoom? result = col.FindOne(x => x.PeerHexPublicKey.Value == p2PPeerRoom.PeerHexPublicKey.Value);

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
            InConnectionNullable(CollectionName,col =>
            {
                // P2PPeerRoom? peerRoom = col.Query().Where(x => x.P2PUserId == p2PUserId)
                //     .FirstOrDefault();

                P2PPeerRoom? peerRoom = col.FindOne(x => x.P2PUserId == p2PUserId);

                return Task.FromResult(peerRoom ?? null);
            });

        public Task<P2PPeerRoom?> TryRead(HexString peerHexPublicKey) =>
            InConnectionNullable(CollectionName,col =>
            {
                // P2PPeerRoom? peerRoom = col.Query().Where(x => x.PeerHexPublicKey.Value == peerHexPublicKey.Value)
                //     .FirstOrDefault();

                P2PPeerRoom? peerRoom = col.FindOne(x => x.PeerHexPublicKey.Value == peerHexPublicKey.Value);

                return Task.FromResult(peerRoom ?? null);
            });
    }
}