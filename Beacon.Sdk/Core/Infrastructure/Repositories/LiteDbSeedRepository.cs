namespace Beacon.Sdk.Core.Infrastructure.Repositories
{
    using System.Threading.Tasks;
    using Data;
    using Domain.Interfaces.Data;
    using LiteDB;
    using Microsoft.Extensions.Logging;

    // Todo: Add secure storage
    public class LiteDbSeedRepository : BaseLiteDbRepository, ISeedRepository
    {
        private readonly ILogger<LiteDbSeedRepository> _logger;
        private readonly object _syncRoot = new();

        public LiteDbSeedRepository(ILogger<LiteDbSeedRepository> logger, RepositorySettings settings) : base(settings)
        {
            _logger = logger;
        }

        public Task<string?> TryRead()
        {
            lock (_syncRoot)
            {
                using var db = new LiteDatabase(ConnectionString);

                ILiteCollection<SeedData>? col = db.GetCollection<SeedData>(nameof(SeedData));

                SeedData seedData = col.Query().FirstOrDefault();

                return Task.FromResult(seedData?.Seed);
            }

            // throw new Exception("Unknown exception");
        }

        public Task<string> Create(string seed)
        {
            lock (_syncRoot)
            {
                using var db = new LiteDatabase(ConnectionString);

                ILiteCollection<SeedData>? col = db.GetCollection<SeedData>(nameof(SeedData));

                col.Insert(new SeedData
                {
                    Seed = seed
                });

                return Task.FromResult(seed);
            }

            // throw new Exception("Unknown exception");
        }
    }
}