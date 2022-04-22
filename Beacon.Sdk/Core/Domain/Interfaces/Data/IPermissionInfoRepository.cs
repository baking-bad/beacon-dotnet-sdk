namespace Beacon.Sdk.Core.Domain.Interfaces.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Entities;

    public interface IPermissionInfoRepository
    {
        Task<PermissionInfo> CreateOrUpdateAsync(PermissionInfo permissionInfo);

        Task<PermissionInfo?> TryReadAsync(string accountIdentifier);

        Task<List<PermissionInfo>> ReadAllAsync();

        Task DeleteByAddressAsync(string address);
    }
}