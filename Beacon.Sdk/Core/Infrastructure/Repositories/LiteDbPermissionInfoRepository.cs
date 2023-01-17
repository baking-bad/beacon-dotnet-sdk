using System;
using LiteDB;

namespace Beacon.Sdk.Core.Infrastructure.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Domain.Entities;
    using Domain.Interfaces.Data;
    using Microsoft.Extensions.Logging;

    public class LiteDbPermissionInfoRepository : BaseLiteDbRepository<PermissionInfo>, IPermissionInfoRepository
    {
        private const string CollectionName = "PermissionInfo";

        public LiteDbPermissionInfoRepository(ILogger<LiteDbPermissionInfoRepository> logger,
            RepositorySettings settings) : base(logger, settings)
        {
        }

        public async Task<PermissionInfo> CreateOrUpdateAsync(PermissionInfo permissionInfo)
        {
            var func = new Func<LiteCollection<PermissionInfo>, Task<PermissionInfo>>(col =>
            {
                var id = $"{permissionInfo.SenderId}:{permissionInfo.AccountId}";
                permissionInfo.PermissionInfoId = id;
                var oldPermissionInfo = col.FindOne(x => x.AppMetadata.Name == permissionInfo.AppMetadata.Name);

                if (oldPermissionInfo != null)
                    permissionInfo.Id = oldPermissionInfo.Id;

                col.Upsert(permissionInfo);
                col.EnsureIndex(x => x.PermissionInfoId);
                return Task.FromResult(permissionInfo);
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

        public Task<PermissionInfo?> TryReadAsync(string senderId, string accountId) =>
            InConnectionNullable(CollectionName, col =>
            {
                var id = $"{senderId}:{accountId}";
                var permissionInfo = col.FindOne(x => x.PermissionInfoId == id);
                return Task.FromResult(permissionInfo ?? null);
            });
        
        public Task<PermissionInfo?> TryReadBySenderIdAsync(string senderId) =>
            InConnectionNullable(CollectionName, col =>
            {
                var permissionInfo = col.FindOne(x => x.SenderId == senderId);
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
        
        public Task DeleteBySenderIdAsync(string senderId) =>
            InConnectionAction(CollectionName, col =>
            {
                var permissionInfo = col.FindOne(x => x.SenderId == senderId);
                
                if (permissionInfo != null)
                    col.Delete(permissionInfo.Id);
            });
    }
}