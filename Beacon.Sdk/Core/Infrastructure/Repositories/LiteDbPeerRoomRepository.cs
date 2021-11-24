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

    public class LiteDbPeerRoomRepository : BaseLiteDbRepository, IPeerRoomRepository
    {
        private readonly ILogger<LiteDbPeerRoomRepository> _logger;
        private readonly object _syncRoot = new();

        public LiteDbPeerRoomRepository(ILogger<LiteDbPeerRoomRepository> logger, RepositorySettings settings) : base(settings)
        {
            _logger = logger;
        }

        public Task<BeaconPeerRoom?> TryRead(HexString peerPublicKey)
        {
            try
            {
                lock (_syncRoot)
                {
                    using var db = new LiteDatabase(ConnectionString);

                    ILiteCollection<BeaconPeerRoom>? col = db.GetCollection<BeaconPeerRoom>(nameof(SeedData));

                    BeaconPeerRoom? peerRoom = col.Query().Where(x => x.BeaconPeerHexPublicKey.Value == peerPublicKey.Value)
                        .FirstOrDefault();

                    return Task.FromResult(peerRoom);
                }
            }
            catch (Exception ex)
            {
                throw;
                // _logger?.LogError(ex, "Error reading PeerRoom");
            }

            // throw new Exception("Unknown exception");
        }

        public Task<BeaconPeerRoom> CreateOrUpdate(BeaconPeerRoom beaconPeerRoom)
        {
            try
            {
                lock (_syncRoot)
                {
                    using var db = new LiteDatabase(ConnectionString);

                    ILiteCollection<BeaconPeerRoom>? col = db.GetCollection<BeaconPeerRoom>(nameof(SeedData));

                    BeaconPeerRoom? result = col.Query()
                        .Where(x => x.BeaconPeerHexPublicKey.Value == beaconPeerRoom.BeaconPeerHexPublicKey.Value).FirstOrDefault();

                    if (result != null)
                    {
                        col.Update(beaconPeerRoom);
                    }
                    else
                    {
                        col.Insert(beaconPeerRoom);
                        col.EnsureIndex(x => x.BeaconPeerHexPublicKey);
                    }

                    return Task.FromResult(beaconPeerRoom);
                }
            }
            catch (Exception ex)
            {
                throw;
                // _logger?.LogError(ex, "Error reading PeerRoom");
            }
        }
    }
}