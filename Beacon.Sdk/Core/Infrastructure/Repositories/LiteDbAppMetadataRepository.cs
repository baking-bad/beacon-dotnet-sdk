namespace Beacon.Sdk.Core.Infrastructure.Repositories
{
    using System.Threading.Tasks;
    using Beacon;
    using Microsoft.Extensions.Logging;

    public class LiteDbAppMetadataRepository : BaseLiteDbRepository<AppMetadata>, IAppMetadataRepository
    {
        public LiteDbAppMetadataRepository(ILogger<LiteDbAppMetadataRepository> logger, RepositorySettings settings)
            : base(logger, settings)
        {
        }

        public Task<AppMetadata> CreateOrUpdate(AppMetadata appMetadata) =>
            InConnection(col =>
            {
                AppMetadata? result = col.Query()
                    .Where(x => x.SenderId == appMetadata.SenderId)
                    .FirstOrDefault();

                if (result == null)
                {
                    col.Insert(appMetadata);
                    col.EnsureIndex(x => x.SenderId);
                }
                else
                {
                    col.Update(appMetadata);
                }

                return Task.FromResult(appMetadata);
            });

        public Task<AppMetadata?> TryRead(string senderId) =>
            InConnectionNullable(col =>
            {
                AppMetadata? appMetadata = col.Query().Where(x => x.SenderId == senderId)
                    .FirstOrDefault();

                return Task.FromResult(appMetadata ?? null);
            });

        public Task<AppMetadata[]?> ReadAll() =>
            InConnectionNullable(col =>
            {
                AppMetadata[]? result = col.Query().ToArray();

                return Task.FromResult(result ?? null);
            });

        public Task Delete(string senderId) =>
            InConnection(col =>
            {
                col.Delete(senderId);

                return (Task<AppMetadata>) Task.CompletedTask;
            });

        public Task DeleteAll() =>
            InConnection(col =>
            {
                col.DeleteAll();

                return (Task<AppMetadata>) Task.CompletedTask;
            });
    }
}