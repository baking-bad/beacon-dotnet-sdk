namespace Beacon.Sdk.Core.Infrastructure.Repositories
{
    using System.Threading.Tasks;
    using Domain.Entities;
    using Domain.Interfaces.Data;
    using Microsoft.Extensions.Logging;

    public class LiteDbPermissionInfoRepository : BaseLiteDbRepository<PermissionInfo>, IPermissionInfoRepository
    {
        public LiteDbPermissionInfoRepository(ILogger<LiteDbPermissionInfoRepository> logger,
            RepositorySettings settings) : base(logger, settings)
        {
        }

        public Task<PermissionInfo> CreateOrUpdate(PermissionInfo permissionInfo) => InConnection(col =>
        {
            col.Upsert(permissionInfo);
            col.EnsureIndex(x => x.AccountIdentifier);

            return Task.FromResult(permissionInfo);
        });

        public Task<PermissionInfo?> TryRead(string accountIdentifier) => InConnectionNullable(col =>
        {
            // PermissionInfo? permissionInfo =
            //     col.Query().Where(x => x.AccountIdentifier == accountIdentifier).FirstOrDefault();

            PermissionInfo? permissionInfo = col.FindOne(x => x.AccountIdentifier == accountIdentifier);

            return Task.FromResult(permissionInfo ?? null);
        });
    }
}