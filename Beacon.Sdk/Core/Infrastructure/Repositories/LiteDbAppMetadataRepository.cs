namespace Beacon.Sdk.Core.Infrastructure.Repositories
{
    using System.Linq;
    using System.Threading.Tasks;
    using Beacon;
    using LiteDB;
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
                AppMetadata? result = col.FindOne(x => x.SenderId == appMetadata.SenderId);
                    // .Where(x => x.SenderId == appMetadata.SenderId)
                    // .FirstOrDefault();

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
                AppMetadata? appMetadata = col.FindOne(x => x.SenderId == senderId);
                // AppMetadata? appMetadata = col.Query().Where(x => x.SenderId == senderId)
                //     .FirstOrDefault();

                return Task.FromResult(appMetadata ?? null);
            });

        public Task<AppMetadata[]?> ReadAll() =>
            InConnectionNullable(col =>
            {
                AppMetadata[]? result = col.FindAll().ToArray();
                // AppMetadata[]? result = col.Query().ToArray();

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
                col.Delete(Query.All());
                // col.DeleteAll();

                return (Task<AppMetadata>) Task.CompletedTask;
            });
    }
}