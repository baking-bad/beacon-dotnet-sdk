namespace Beacon.Sdk.Core.Infrastructure.Repositories
{
    using System;
    using System.Threading.Tasks;
    using Domain.Entities;
    using Domain.Interfaces.Data;
    using Microsoft.Extensions.Logging;

    public class LiteDbPeerRepository : BaseLiteDbRepository<Peer>, IPeerRepository
    {
        private const string CollectionName = "Peer";
        
        public LiteDbPeerRepository(ILogger<LiteDbPeerRepository> logger, RepositorySettings settings) :
            base(logger, settings)
        {
        }

        public Task<Peer> CreateAsync(Peer peer) =>
            InConnection(CollectionName, col =>
            {
                try
                {
                    col.Insert(peer);
                    col.EnsureIndex(x => x.SenderUserId);

                    return Task.FromResult(peer);
                } catch (Exception ex)
                {
                    throw ex;
                }
            });

        public Task<Peer?> TryReadAsync(string senderUserId) =>
            InConnectionNullable(CollectionName,col =>
            {
                // Peer? peer = col.Query().Where(x => x.SenderUserId == senderUserId).FirstOrDefault();

                col.EnsureIndex(x => x.SenderUserId);
                
                Peer? peer = col.FindOne(x => x.SenderUserId == senderUserId);

                return Task.FromResult(peer ?? null);
            });
    }
}