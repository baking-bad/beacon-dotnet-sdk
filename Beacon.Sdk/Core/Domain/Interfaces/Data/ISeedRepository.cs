namespace Beacon.Sdk.Core.Domain.Interfaces.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Entities;

    public interface ISeedRepository
    {
        Task<SeedEntity> CreateAsync(string seed);

        Task<SeedEntity?> TryReadAsync();

        Task<List<SeedEntity>> ReadAllAsync();
    }
}