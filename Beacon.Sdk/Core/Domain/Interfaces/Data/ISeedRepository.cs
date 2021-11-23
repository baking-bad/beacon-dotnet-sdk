namespace Beacon.Sdk.Core.Domain.Interfaces.Data
{
    using System.Threading.Tasks;

    public interface ISeedRepository
    {
        Task<string?> TryRead();

        Task<string> Create(string seed);
    }
}