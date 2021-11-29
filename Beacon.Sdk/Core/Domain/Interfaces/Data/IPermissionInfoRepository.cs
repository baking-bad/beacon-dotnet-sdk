namespace Beacon.Sdk.Core.Domain.Interfaces.Data
{
    using System.Threading.Tasks;
    using global::Beacon.Sdk.Core.Domain.Entities;

    public interface IPermissionInfoRepository
    {
        Task<PermissionInfo> Create(PermissionInfo permissionInfo);

        Task<PermissionInfo?> TryRead(string accountIdentifier);
    }
}