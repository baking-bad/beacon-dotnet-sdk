namespace Beacon.Sdk.Core.Domain.Interfaces.Data
{
    using System.Threading.Tasks;
    using Entities;

    public interface ISeedRepository
    {
        Task<SeedEntity> Create(string seed);

        Task<SeedEntity?> TryRead();
    }
}