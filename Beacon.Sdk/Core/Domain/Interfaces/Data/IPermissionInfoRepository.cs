namespace Beacon.Sdk.Core.Domain.Interfaces.Data
{
    using System.Threading.Tasks;
    using Entities;

    public interface IPermissionInfoRepository
    {
        Task<PermissionInfo> CreateOrUpdate(PermissionInfo permissionInfo);

        Task<PermissionInfo?> TryRead(string accountIdentifier);
    }
}