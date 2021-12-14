namespace Beacon.Sdk.Core.Domain
{
    using System.Threading.Tasks;
    using Beacon;
    using Beacon.Operation;
    using Beacon.Permission;
    using Entities;
    using Interfaces.Data;
    using Services;

    public class PermissionHandler
    {
        private readonly IPermissionInfoRepository _permissionInfoRepository;
        private readonly AccountService _accountService;

        public PermissionHandler(IPermissionInfoRepository permissionInfoRepository, AccountService accountService)
        {
            _permissionInfoRepository = permissionInfoRepository;
            _accountService = accountService;
        }

        public async Task<bool> HasPermission(IBeaconRequest beaconRequest) =>
            beaconRequest.Type switch
            {
                BeaconMessageType.permission_request => true,
                BeaconMessageType.broadcast_request => true,
                BeaconMessageType.operation_request => await HandleOperationRequest(beaconRequest),
                _ => false
            };

        private async Task<bool> HandleOperationRequest(IBeaconRequest request)
        {
            var operationRequest = request as OperationRequest;
            string accountIdentifier = _accountService.GetAccountIdentifier(operationRequest!.SourceAddress, operationRequest.Network);

            PermissionInfo? permissionInfo = await _permissionInfoRepository.TryRead(accountIdentifier);
            
            return permissionInfo != null && permissionInfo.Scopes.Contains(PermissionScope.operation_request);
        }
    }
}