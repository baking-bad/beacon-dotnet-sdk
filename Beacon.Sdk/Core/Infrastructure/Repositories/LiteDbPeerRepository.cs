namespace Beacon.Sdk.Core.Infrastructure.Repositories
{
    using System.Linq;
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

        public Task<Peer> Create(Peer peer)
        {
            lock (_syncRoot)
            {
                using var db = new LiteDatabase(ConnectionString);

                ILiteCollection<Peer>? col = db.GetCollection<Peer>(nameof(Peer));

                col.Insert(peer);

                col.EnsureIndex(x => x.SenderUserId);

                return Task.FromResult(peer);
            }

            // throw new Exception("Unknown exception");
        }

        public Task<Peer?> TryReadBySenderUserId(string senderUserId)
        {
            lock (_syncRoot)
            {
                using var db = new LiteDatabase(ConnectionString);

                ILiteCollection<Peer>? col = db.GetCollection<Peer>(nameof(Peer));

                Peer? peer = col.Query().Where(x => x.SenderUserId == senderUserId).FirstOrDefault();

                return Task.FromResult(peer);
            }

            // throw new Exception("Unknown exception");
        }
    }
}