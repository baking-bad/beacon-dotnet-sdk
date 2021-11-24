namespace Beacon.Sdk.Core.Infrastructure.Repositories
{
    using System.Threading.Tasks;
    using Data;
    using Domain;
    using Domain.Interfaces.Data;
    using LiteDB;
    using Microsoft.Extensions.Logging;
    using Utils;

    public class LiteDbPeerRoomRepository : BaseLiteDbRepository, IPeerRoomRepository
    {
        private readonly ILogger<LiteDbPeerRoomRepository> _logger;
        private readonly object _syncRoot = new();

        public LiteDbPeerRoomRepository(ILogger<LiteDbPeerRoomRepository> logger, RepositorySettings settings) :
            base(settings)
        {
            _logger = logger;
        }

        public Task<PeerRoom?> TryRead(HexString peerPublicKey)
        {
            lock (_syncRoot)
            {
                using var db = new LiteDatabase(ConnectionString);

                ILiteCollection<PeerRoom>? col = db.GetCollection<PeerRoom>(nameof(SeedData));

                PeerRoom? peerRoom = col.Query().Where(x => x.PeerHexPublicKey.Value == peerPublicKey.Value)
                    .FirstOrDefault();

                return Task.FromResult(peerRoom);
            }

            // throw new Exception("Unknown exception");
        }

        public Task<PeerRoom> CreateOrUpdate(PeerRoom peerRoom)
        {
            lock (_syncRoot)
            {
                using var db = new LiteDatabase(ConnectionString);

                ILiteCollection<PeerRoom>? col = db.GetCollection<PeerRoom>(nameof(SeedData));

                PeerRoom? result = col.Query()
                    .Where(x => x.PeerHexPublicKey.Value == peerRoom.PeerHexPublicKey.Value)
                    .FirstOrDefault();

                if (result != null)
                {
                    col.Update(peerRoom);
                }
                else
                {
                    col.Insert(peerRoom);
                    col.EnsureIndex(x => x.PeerHexPublicKey);
                }

                return Task.FromResult(peerRoom);
            }
        }
    }
}