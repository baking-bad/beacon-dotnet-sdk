namespace Beacon.Sdk.Core.Infrastructure.Repositories
{
    using System;
    using System.Threading.Tasks;
    using Data;
    using Domain;
    using Domain.Interfaces.Data;
    using LiteDB;
    using Microsoft.Extensions.Logging;
    using Utils;

    public class LiteDbPeerRoomRepository : IPeerRoomRepository
    {
        private readonly ILogger<LiteDbPeerRoomRepository> _logger;
        private readonly object _syncRoot = new();

        public LiteDbPeerRoomRepository(ILogger<LiteDbPeerRoomRepository> logger)
        {
            _logger = logger;
        }

        public Task<PeerRoom?> TryRead(HexString peerPublicKey)
        {
            try
            {
                lock (_syncRoot)
                {
                    using var db = new LiteDatabase(Constants.ConnectionString);

                    ILiteCollection<PeerRoom>? col = db.GetCollection<PeerRoom>(nameof(SeedData));

                    PeerRoom? peerRoom = col.Query().Where(x => x.PeerHexPublicKey.Value == peerPublicKey.Value)
                        .FirstOrDefault();

                    return Task.FromResult(peerRoom);
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error reading PeerRoom");
            }

            throw new Exception("Unknown exception");
        }

        public Task<PeerRoom> CreateOrUpdate(PeerRoom peerRoom)
        {
            try
            {
                lock (_syncRoot)
                {
                    using var db = new LiteDatabase(Constants.ConnectionString);

                    ILiteCollection<PeerRoom>? col = db.GetCollection<PeerRoom>(nameof(SeedData));

                    PeerRoom? result = col.Query()
                        .Where(x => x.PeerHexPublicKey.Value == peerRoom.PeerHexPublicKey.Value).FirstOrDefault();

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
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error reading PeerRoom");
            }

            throw new Exception("Unknown exception");
        }
    }
}