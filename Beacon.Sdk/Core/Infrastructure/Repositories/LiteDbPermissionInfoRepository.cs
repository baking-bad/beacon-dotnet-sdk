namespace Beacon.Sdk.Core.Infrastructure.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Domain.Entities;
    using Domain.Interfaces.Data;
    using LiteDB;
    using Microsoft.Extensions.Logging;

    public class LiteDbPermissionInfoRepository : BaseLiteDbRepository<PermissionInfo>, IPermissionInfoRepository
    {
        private const string CollectionName = "PermissionInfo";

        public LiteDbPermissionInfoRepository(ILogger<LiteDbPermissionInfoRepository> logger,
            RepositorySettings settings) : base(logger, settings)
        {
        }

        public Task<PermissionInfo> CreateOrUpdateAsync(PermissionInfo permissionInfo) =>
            InConnection(CollectionName, col =>
            {
                var id = $"{permissionInfo.SenderId}:{permissionInfo.AccountId}";
                permissionInfo.PermissionInfoId = id;
                
                PermissionInfo? oldPermissionInfo = col.FindOne(x => x.PermissionInfoId == id);

                if (oldPermissionInfo != null)
                    col.Update(permissionInfo);
                else
                    col.Insert(permissionInfo);

                col.EnsureIndex(x => x.PermissionInfoId);

                return Task.FromResult(permissionInfo);
            });

        public Task<PermissionInfo?> TryReadAsync(string senderId, string accountId) =>
            InConnectionNullable(CollectionName, col =>
            {
                var id = $"{senderId}:{accountId}";
                
                col.EnsureIndex(x => x.PermissionInfoId);
                
                PermissionInfo? permissionInfo = col.FindOne(x => x.PermissionInfoId == id);

                return Task.FromResult(permissionInfo ?? null);
            });

        public Task<List<PermissionInfo>> ReadAllAsync() =>
            InConnection(CollectionName, col =>
            {
                var result = col.FindAll().ToList();

                return Task.FromResult(result);
            });

        public Task DeleteByIdAsync(string id) =>
            InConnectionAction(CollectionName, col =>
            {
                col.EnsureIndex(x => x.PermissionInfoId);
                
                PermissionInfo? permissionInfo = col.FindOne(x => x.PermissionInfoId == id);
                if (permissionInfo != null)
                    col.Delete(permissionInfo.Id);
            });
    }
}