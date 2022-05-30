namespace Beacon.Sdk.Core.Domain
{
    using System.Threading.Tasks;
    using Beacon;
    using Beacon.Operation;
    using Entities;
    using Interfaces.Data;
    using Services;

    public class PermissionHandler
    {
        private readonly AccountService _accountService;
        private readonly IPermissionInfoRepository _permissionInfoRepository;

        public PermissionHandler(IPermissionInfoRepository permissionInfoRepository, AccountService accountService)
        {
            _permissionInfoRepository = permissionInfoRepository;
            _accountService = accountService;
        }

        public async Task<bool> HasPermission(BaseBeaconMessage beaconRequest) =>
            beaconRequest.Type switch
            {
                BeaconMessageType.permission_request => true,
                BeaconMessageType.broadcast_request => true,
                BeaconMessageType.operation_request => await HandleOperationRequest(beaconRequest as OperationRequest),
                _ => false
            };

        private async Task<bool> HandleOperationRequest(OperationRequest request)
        {
            string accountId = _accountService.GetAccountId(request.SourceAddress, request.Network);

            PermissionInfo? permissionInfo = await _permissionInfoRepository.TryReadAsync(request.SenderId, accountId);

            return permissionInfo != null; // && permissionInfo.Scopes.Contains(PermissionScope.operation_request);
        }
    }
}