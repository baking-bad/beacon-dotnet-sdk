namespace Beacon.Sdk.Core.Infrastructure.Repositories
{
    using System.Linq;
    using System.Threading.Tasks;
    using Beacon;
    using Microsoft.Extensions.Logging;

    public class LiteDbAppMetadataRepository : BaseLiteDbRepository<AppMetadata>, IAppMetadataRepository
    {
        private const string CollectionName = "AppMetadata";

        public LiteDbAppMetadataRepository(ILogger<LiteDbAppMetadataRepository> logger, RepositorySettings settings)
            : base(logger, settings)
        {
        }

        public Task<AppMetadata> CreateOrUpdateAsync(AppMetadata appMetadata) =>
            InConnection(CollectionName, col =>
            {
                var result = col.FindOne(x => x.Name == appMetadata.Name);
                if (result != null)
                    appMetadata.Id = result.Id;

                col.Upsert(appMetadata);
                col.EnsureIndex(x => x.SenderId);
                return Task.FromResult(appMetadata);
            });

        public Task<AppMetadata?> TryReadAsync(string senderId) =>
            InConnectionNullable(CollectionName, col =>
            {
                col.EnsureIndex(x => x.SenderId);

                AppMetadata? appMetadata = col.FindOne(x => x.SenderId == senderId);
                // AppMetadata? appMetadata = col.Query().Where(x => x.SenderId == senderId)
                //     .FirstOrDefault();

                return Task.FromResult(appMetadata ?? null);
            });

        public Task<AppMetadata[]?> ReadAll() =>
            InConnectionNullable(CollectionName, col =>
            {
                AppMetadata[]? result = col.FindAll().ToArray();
                return Task.FromResult(result ?? null);
            });

        public Task Delete(string senderId) => InConnectionAction(CollectionName, col =>
        {
            var appMetaData = col.FindOne(a => a.SenderId == senderId);

            if (appMetaData != null)
                col.Delete(senderId);
        });

        public Task DeleteAll() =>
            InConnection(CollectionName, col =>
            {
                // col.Delete(Query.All());
                // col.DeleteAll();

                return (Task<AppMetadata>)Task.CompletedTask;
            });
    }
}