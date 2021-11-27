namespace Beacon.Sdk.Core.Domain.Interfaces
{
    using System.Threading.Tasks;
    using Infrastructure.Repositories;

    public interface IPermissionInfoRepository
    {
        Task<PermissionInfo> Create(PermissionInfo permissionInfo);
        
        Task<PermissionInfo?> TryRead(string accountIdentifier);
    }
}