using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using LiteDB;
using Microsoft.Extensions.Logging;

namespace Beacon.Sdk.Core.Infrastructure.Repositories
{
    using Domain.Entities;
    using Domain.Interfaces.Data;

    public class LiteDbPeerRepository : BaseLiteDbRepository<Peer>, IPeerRepository
    {
        private const string CollectionName = "Peer";

        public LiteDbPeerRepository(
            ILiteDbConnectionPool connectionPool,
            ILogger<LiteDbPeerRepository> logger,
            RepositorySettings settings)
            : base(connectionPool, logger, settings)
        {
        }

        public async Task<Peer> CreateAsync(Peer peer)
        {
            var func = new Func<ILiteCollection<Peer>, Task<Peer>>(col =>
            {
                var existingPeer = col.FindOne(p => p.Name == peer.Name);
                if (existingPeer != null)
                    peer.Id = existingPeer.Id;

                col.Upsert(peer);
                col.EnsureIndex(x => x.SenderId);
                return Task.FromResult(peer);
            });
            
            try
            {
                return await InConnection(CollectionName, func);
            }
            catch
            {
                await DropAsync(CollectionName);

                return await InConnection(CollectionName, func);
            }
        }

        public Task<Peer?> TryReadAsync(string senderId) => InConnectionNullable(CollectionName, col =>
        {
            var peer = col.FindOne(x => x.SenderId == senderId);
            return Task.FromResult(peer ?? null);
        });

        public Task<Peer?> TryGetActive() => InConnectionNullable(CollectionName, col =>
        {
            var peer = col.FindOne(x => x.IsActive);
            return Task.FromResult(peer ?? null);
        });

        public Task<List<Peer>> GetAll() =>
            InConnection(CollectionName, col => Task.FromResult(new List<Peer>(col.FindAll())));

        public Task Delete(Peer peer) => InConnectionAction(CollectionName, col =>
        {
            var dbPeer = col.FindOne(x => x.SenderId == peer.SenderId);

            if (dbPeer != null)
                col.Delete(dbPeer.Id);
        });
        
        public Task MarkAllInactive() => InConnectionAction(CollectionName, col =>
        {
            var allPeers = col.FindAll();
            foreach (var peer in allPeers)
            {
                peer.IsActive = false;
                col.Update(peer);
            }
        });
    }
}