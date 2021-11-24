namespace Beacon.Sdk.Core.Infrastructure.Repositories
{
    using System.Threading.Tasks;
    using Domain;
    using Domain.Interfaces.Data;
    using LiteDB;
    using Microsoft.Extensions.Logging;

    public class LiteDbPeerRepository : BaseLiteDbRepository, IPeerRepository
    {
        private readonly ILogger<LiteDbPeerRepository> _logger;
        private readonly object _syncRoot = new();

        public LiteDbPeerRepository(ILogger<LiteDbPeerRepository> logger, RepositorySettings settings) :
            base(settings)
        {
            _logger = logger;
        }

        //  BeaconPeer beaconPeer = BeaconPeer.Factory.Create(_cryptographyService, name, relayServer,hexPublicKey, version);
        public Task<Peer> Create(Peer peer)
        {
            lock (_syncRoot)
            {
                using var db = new LiteDatabase(ConnectionString);

                ILiteCollection<Peer>? col = db.GetCollection<Peer>(nameof(Peer));

                col.Insert(peer);

                col.EnsureIndex(x => x.UserId);

                return Task.FromResult(peer);
            }

            // throw new Exception("Unknown exception");
        }

        public Task<Peer?> TryReadByUserId(string userId)
        {
            lock (_syncRoot)
            {
                using var db = new LiteDatabase(ConnectionString);

                ILiteCollection<Peer>? col = db.GetCollection<Peer>(nameof(Peer));

                Peer? peer = col.Query().Where(x => x.UserId == userId).FirstOrDefault();

                return Task.FromResult(peer);
            }

            // throw new Exception("Unknown exception");
        }
    }
}