using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using LiteDB;
using Microsoft.Extensions.Logging;

namespace Beacon.Sdk.Core.Infrastructure.Repositories
{
    using Domain.Entities.P2P;
    using Domain.Interfaces.Data;
    using Utils;

    public class LiteDbP2PPeerRoomRepository : BaseLiteDbRepository<P2PPeerRoom>, IP2PPeerRoomRepository
    {
        private const string CollectionName = "P2PPeerRoom";

        public LiteDbP2PPeerRoomRepository(
            ILiteDbConnectionPool connectionPool,
            ILogger<LiteDbP2PPeerRoomRepository> logger,
            RepositorySettings settings) :
            base(connectionPool, logger, settings)
        {
        }

        public async Task<P2PPeerRoom> CreateOrUpdateAsync(P2PPeerRoom p2PPeerRoom)
        {
            var func = new Func<ILiteCollection<P2PPeerRoom>, Task<P2PPeerRoom>>(col =>
            {
                var result = col.FindOne(x => x.PeerName == p2PPeerRoom.PeerName);
                if (result != null)
                    p2PPeerRoom.Id = result.Id;

                col.Upsert(p2PPeerRoom);
                col.EnsureIndex(x => x.P2PUserId);
                col.EnsureIndex(x => x.PeerHexPublicKey);

                return Task.FromResult(p2PPeerRoom);
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

        public Task<List<P2PPeerRoom>> GetAll() => InConnection(CollectionName,
            col => Task.FromResult(new List<P2PPeerRoom>(col.FindAll())));

        public Task Remove(HexString peerHexPublicKey) => InConnectionAction(CollectionName, col =>
        {
            var peerRoom = col.FindOne(x => x.PeerHexPublicKey.Value == peerHexPublicKey.Value);

            if (peerRoom != null)
                col.Delete(peerRoom.Id);
        });

        public Task<P2PPeerRoom?> TryReadAsync(string p2PUserId) =>
            InConnectionNullable(CollectionName, col =>
            {
                var peerRoom = col.FindOne(x => x.P2PUserId == p2PUserId);
                return Task.FromResult(peerRoom ?? null);
            });

        public Task<P2PPeerRoom?> TryReadAsync(HexString peerHexPublicKey) =>
            InConnectionNullable(CollectionName, col =>
            {
                P2PPeerRoom? peerRoom = col.FindOne(x => x.PeerHexPublicKey.Value == peerHexPublicKey.Value);
                return Task.FromResult(peerRoom ?? null);
            });
    }
}