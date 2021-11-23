namespace Beacon.Sdk.Core.Infrastructure.Repositories
{
    using System;
    using System.Threading.Tasks;
    using Data;
    using Domain.Interfaces.Data;
    using LiteDB;
    using Microsoft.Extensions.Logging;

    // Todo: Add secure storage
    public class LiteDbSeedRepository : ISeedRepository
    {
        private readonly ILogger<LiteDbSeedRepository>? _logger;
        private readonly object _syncRoot = new();
        
        public LiteDbSeedRepository(ILogger<LiteDbSeedRepository> logger)
        {
            _logger = logger;
        }
        
        public Task<string?> TryRead()
        {
            try
            {
                lock (_syncRoot)
                {
                    using var db = new LiteDatabase(Constants.ConnectionString);
                
                    ILiteCollection<SeedData>? col = db.GetCollection<SeedData>(nameof(SeedData));

                    SeedData seedData = col.Query().FirstOrDefault();

                    return Task.FromResult(seedData?.Seed);    
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error getting KeyPair");
            }

            return Task.FromResult((string?) null);
        }

        public Task<string?> TryCreate(string seed)
        {
            try
            {
                lock (_syncRoot)
                {
                    using var db = new LiteDatabase(Constants.ConnectionString);
                
                    ILiteCollection<SeedData>? col = db.GetCollection<SeedData>(nameof(SeedData));

                    var seedData = new SeedData
                    {
                        Seed = seed
                    };
                    
                    col.Insert(seedData);

                    return Task.FromResult(seedData.Seed)!;
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error creating KeyPair");
            }

            return Task.FromResult((string?) null);
        }
    }
}