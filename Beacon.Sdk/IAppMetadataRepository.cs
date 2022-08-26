namespace Beacon.Sdk
{
    using System.Threading.Tasks;
    using Beacon;

    public interface IAppMetadataRepository
    {
        Task<AppMetadata> CreateOrUpdateAsync(AppMetadata appMetadata);

        Task<AppMetadata?> TryReadAsync(string senderId);

        Task<AppMetadata[]?> ReadAll();

        Task Delete(string senderId);
    }
}