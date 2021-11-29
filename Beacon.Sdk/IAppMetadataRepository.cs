namespace Beacon.Sdk
{
    using System.Threading.Tasks;
    using Beacon;

    public interface IAppMetadataRepository
    {
        Task<AppMetadata> CreateOrUpdate(AppMetadata appMetadata);

        Task<AppMetadata?> TryRead(string senderId);

        Task<AppMetadata[]?> ReadAll(string senderId);

        Task Delete(string senderId);
    }
}