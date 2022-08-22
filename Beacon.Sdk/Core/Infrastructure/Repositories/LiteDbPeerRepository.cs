using System.Collections.Generic;


namespace Beacon.Sdk.Core.Infrastructure.Repositories
{
    using System.Threading.Tasks;
    using Domain.Entities;
    using Domain.Interfaces.Data;
    using Microsoft.Extensions.Logging;

    public class LiteDbPeerRepository : BaseLiteDbRepository<Peer>, IPeerRepository
    {
        private const string CollectionName = "Peer";

        public LiteDbPeerRepository(ILogger<LiteDbPeerRepository> logger, RepositorySettings settings)
            : base(logger, settings)
        {
        }

        public Task<Peer> CreateAsync(Peer peer) => InConnection(CollectionName, col =>
        {
            var existingPeer = col.FindOne(p => p.Name == peer.Name);
            if (existingPeer != null)
                peer.Id = existingPeer.Id;

            col.Upsert(peer);
            col.EnsureIndex(x => x.SenderId);
            return Task.FromResult(peer);
        });

        public Task<Peer?> TryReadAsync(string senderUserId) => InConnectionNullable(CollectionName, col =>
        {
            var peer = col.FindOne(x => x.SenderId == senderUserId);
            return Task.FromResult(peer ?? null);
        });

        public Task<List<Peer>> GetAll() =>
            InConnection(CollectionName, col => Task.FromResult(new List<Peer>(col.FindAll())));
    }
}