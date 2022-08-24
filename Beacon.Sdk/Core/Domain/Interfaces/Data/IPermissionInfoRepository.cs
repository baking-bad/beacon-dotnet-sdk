namespace Beacon.Sdk.Core.Domain.Interfaces.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Entities;

    public interface IPermissionInfoRepository
    {
        Task<PermissionInfo> CreateOrUpdateAsync(PermissionInfo permissionInfo);

        Task<PermissionInfo?> TryReadAsync(string senderId, string accountId);
        
        Task<PermissionInfo?> TryReadBySenderIdAsync(string senderId);
        
        Task<List<PermissionInfo>> ReadAllAsync();

        Task DeleteByIdAsync(string id);

        Task DeleteBySenderIdAsync(string senderId);
    }
}