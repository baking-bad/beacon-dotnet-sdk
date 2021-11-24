namespace Beacon.Sdk.Core.Infrastructure.Repositories
{
    using System;
    using System.Threading.Tasks;
    using Domain;
    using Domain.Interfaces.Data;
    using LiteDB;
    using Microsoft.Extensions.Logging;

    public class LiteDbBeaconPeerRepository : BaseLiteDbRepository, IBeaconPeerRepository
    {
        private readonly ILogger<LiteDbBeaconPeerRepository> _logger;
        private readonly object _syncRoot = new();

        public LiteDbBeaconPeerRepository(ILogger<LiteDbBeaconPeerRepository> logger, RepositorySettings settings) : base(settings)
        {
            _logger = logger;
        }

        //  BeaconPeer beaconPeer = BeaconPeer.Factory.Create(_cryptographyService, name, relayServer,hexPublicKey, version);
        public Task<BeaconPeer> Create(BeaconPeer peer)
        {
            try
            {
                lock (_syncRoot)
                {
                    using var db = new LiteDatabase(ConnectionString);

                    ILiteCollection<BeaconPeer>? col = db.GetCollection<BeaconPeer>(nameof(BeaconPeer));

                    col.Insert(peer);

                    col.EnsureIndex(x => x.UserId);

                    return Task.FromResult(peer);
                }
            }
            catch (Exception ex)
            {
                throw;
                // _logger?.LogError(ex, "Error creating BeaconPeerData");
            }

            // throw new Exception("Unknown exception");
        }

        public Task<BeaconPeer?> TryReadByUserId(string userId)
        {
            try
            {
                lock (_syncRoot)
                {
                    using var db = new LiteDatabase(ConnectionString);

                    ILiteCollection<BeaconPeer>? col = db.GetCollection<BeaconPeer>(nameof(BeaconPeer));

                    BeaconPeer? peer = col.Query().Where(x => x.UserId == userId).FirstOrDefault();

                    return Task.FromResult(peer);
                }
            }
            catch (Exception ex)
            {
                throw;
                // _logger?.LogError(ex, "Error reading BeaconPeerData");
            }

            // throw new Exception("Unknown exception");
        }
    }
}