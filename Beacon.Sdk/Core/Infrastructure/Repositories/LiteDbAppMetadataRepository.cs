using System;
using System.Linq;
using System.Threading.Tasks;

using LiteDB;
using Microsoft.Extensions.Logging;

namespace Beacon.Sdk.Core.Infrastructure.Repositories
{
    using Beacon;

    public class LiteDbAppMetadataRepository : BaseLiteDbRepository<AppMetadata>, IAppMetadataRepository
    {
        private const string CollectionName = "AppMetadata";

        public LiteDbAppMetadataRepository(
            ILiteDbConnectionPool connectionPool,
            ILogger<LiteDbAppMetadataRepository> logger,
            RepositorySettings settings)
            : base(connectionPool, logger, settings)
        {
        }

        public async Task<AppMetadata> CreateOrUpdateAsync(AppMetadata appMetadata)
        {
            var func = new Func<ILiteCollection<AppMetadata>, Task<AppMetadata>>(col =>
            {
                var result = col.FindOne(x => x.Name == appMetadata.Name);
                if (result != null)
                    appMetadata.Id = result.Id;

                col.Upsert(appMetadata);
                col.EnsureIndex(x => x.SenderId);
                return Task.FromResult(appMetadata);
            });

            try
            {
                return await InConnection(CollectionName, func);
            }
            catch
            {
                await DropAsync(CollectionName);

                return await InConnection(CollectionName, func);
            }
        }

        public Task<AppMetadata?> TryReadAsync(string senderId) =>
            InConnectionNullable(CollectionName, col =>
            {
                var appMetadata = col.FindOne(x => x.SenderId == senderId);
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
                col.Delete(appMetaData.Id);
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