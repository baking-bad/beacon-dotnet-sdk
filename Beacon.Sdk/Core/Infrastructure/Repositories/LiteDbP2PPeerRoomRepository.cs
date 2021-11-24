namespace Beacon.Sdk.Core.Infrastructure.Repositories
{
    using System.Threading.Tasks;
    using Data;
    using Domain;
    using Domain.Interfaces.Data;
    using Domain.P2P;
    using LiteDB;
    using Microsoft.Extensions.Logging;
    using Utils;

    public class LiteDbP2PPeerRoomRepository : BaseLiteDbRepository, IP2PPeerRoomRepository
    {
        private readonly ILogger<LiteDbP2PPeerRoomRepository> _logger;
        private readonly object _syncRoot = new();

        public LiteDbP2PPeerRoomRepository(ILogger<LiteDbP2PPeerRoomRepository> logger, RepositorySettings settings) :
            base(settings)
        {
            _logger = logger;
        }

        public Task<P2PPeerRoom?> TryReadByP2PUserId(string p2PUserId)
        {
            lock (_syncRoot)
            {
                using var db = new LiteDatabase(ConnectionString);

                ILiteCollection<P2PPeerRoom>? col = db.GetCollection<P2PPeerRoom>(nameof(SeedData));

                P2PPeerRoom? peerRoom = col.Query().Where(x => x.P2PUserId == p2PUserId)
                    .FirstOrDefault();

                return Task.FromResult(peerRoom);
            }

            // throw new Exception("Unknown exception");
        }
        
        public Task<P2PPeerRoom?> TryReadByPeerHexPublicKey(HexString peerHexPublicKey)
        {
            lock (_syncRoot)
            {
                using var db = new LiteDatabase(ConnectionString);

                ILiteCollection<P2PPeerRoom>? col = db.GetCollection<P2PPeerRoom>(nameof(SeedData));

                P2PPeerRoom? peerRoom = col.Query().Where(x => x.PeerHexPublicKey.Value == peerHexPublicKey.Value)
                    .FirstOrDefault();

                return Task.FromResult(peerRoom);
            }

            // throw new Exception("Unknown exception");
        }

        public Task<P2PPeerRoom> CreateOrUpdate(P2PPeerRoom p2PPeerRoom)
        {
            lock (_syncRoot)
            {
                using var db = new LiteDatabase(ConnectionString);

                ILiteCollection<P2PPeerRoom>? col = db.GetCollection<P2PPeerRoom>(nameof(SeedData));

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
                    col.Update(p2PPeerRoom);

                return Task.FromResult(p2PPeerRoom);
            }
        }
    }
}