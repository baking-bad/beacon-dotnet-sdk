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

        public Task<Peer> CreateAsync(Peer peer) =>
            InConnection(CollectionName, col =>
            {
                col.Insert(peer);
                col.EnsureIndex(x => x.SenderId);

                return Task.FromResult(peer);
            });

        public Task<Peer?> TryReadAsync(string senderUserId) =>
            InConnectionNullable(CollectionName, col =>
            {
                // Peer? peer = col.Query().Where(x => x.SenderUserId == senderUserId).FirstOrDefault();

                col.EnsureIndex(x => x.SenderId);

                Peer? peer = col.FindOne(x => x.SenderId == senderUserId);

                return Task.FromResult(peer ?? null);
            });
    }
}