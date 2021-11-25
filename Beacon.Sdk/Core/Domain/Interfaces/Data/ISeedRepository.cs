namespace Beacon.Sdk.Core.Domain.Interfaces.Data
{
    using System.Threading.Tasks;

    public interface ISeedRepository
    {
        Task<SeedEntity> Create(string seed);
        
        Task<SeedEntity?> TryRead();
    }
}