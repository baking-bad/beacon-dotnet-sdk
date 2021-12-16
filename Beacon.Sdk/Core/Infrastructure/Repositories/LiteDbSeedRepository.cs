namespace Beacon.Sdk.Core.Infrastructure.Repositories
{
    using System.Linq;
    using System.Threading.Tasks;
    using Domain.Entities;
    using Domain.Interfaces.Data;
    using Microsoft.Extensions.Logging;

    // Todo: Add secure storage
    public class LiteDbSeedRepository : BaseLiteDbRepository<SeedEntity>, ISeedRepository
    {
        public LiteDbSeedRepository(ILogger<LiteDbSeedRepository> logger, RepositorySettings settings)
            : base(logger, settings)
        {
        }

        public Task<SeedEntity> Create(string seed) =>
            InConnection(col =>
            {
                var data = new SeedEntity
                {
                    Seed = seed
                };

                col.Insert(data);
                col.EnsureIndex(x => x.Seed);

                return Task.FromResult(data);
            });

        public Task<SeedEntity?> TryRead() =>
            InConnectionNullable(col =>
            {
                // SeedEntity seedEntity = col.Query().FirstOrDefault();

                // return Task.FromResult(seedEntity ?? null);

                SeedEntity? seedEntity = col.FindAll().FirstOrDefault();

                return Task.FromResult(seedEntity ?? null);
            });
    }
}