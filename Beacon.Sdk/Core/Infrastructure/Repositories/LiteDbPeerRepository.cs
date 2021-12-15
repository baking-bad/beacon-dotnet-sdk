namespace Beacon.Sdk.Core.Infrastructure.Repositories
{
    using System.Threading.Tasks;
    using Domain.Entities;
    using Domain.Interfaces.Data;
    using Microsoft.Extensions.Logging;

    public class LiteDbPeerRepository : BaseLiteDbRepository<Peer>, IPeerRepository
    {
        public LiteDbPeerRepository(ILogger<LiteDbPeerRepository> logger, RepositorySettings settings) :
            base(logger, settings)
        {
        }

        public Task<Peer> Create(Peer peer) =>
            InConnection(col =>
            {
                col.Insert(peer);
                col.EnsureIndex(x => x.SenderUserId);

                return Task.FromResult(peer);
            });

        public Task<Peer?> TryRead(string senderUserId) =>
            InConnectionNullable(col =>
            {
                // Peer? peer = col.Query().Where(x => x.SenderUserId == senderUserId).FirstOrDefault();

                Peer? peer = col.FindOne(x => x.SenderUserId == senderUserId);
                    
                return Task.FromResult(peer ?? null);
            });
    }
}