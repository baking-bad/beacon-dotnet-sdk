using System.Threading.Tasks;

namespace Beacon.Sdk.Core.Domain
{
    using System;
    using Beacon;
    using Beacon.Permission;
    using Entities;
    using Interfaces;
    using Interfaces.Data;

    public class SerializeMessageHandler
    {
        private readonly IAppMetadataRepository _appMetadataRepository;
        private readonly IJsonSerializerService _jsonSerializerService;
        private readonly PermissionInfoFactory _permissionInfoFactory;
        private readonly IPermissionInfoRepository _permissionInfoRepository;

        public SerializeMessageHandler(
            IAppMetadataRepository appMetadataRepository,
            IPermissionInfoRepository permissionInfoRepository,
            IJsonSerializerService jsonSerializerService,
            PermissionInfoFactory permissionInfoFactory
        )
        {
            _appMetadataRepository = appMetadataRepository;
            _permissionInfoRepository = permissionInfoRepository;
            _jsonSerializerService = jsonSerializerService;
            _permissionInfoFactory = permissionInfoFactory;
        }

        public event EventHandler<ConnectedClientsListChangedEventArgs> OnPermissionsCreated;

        public async Task<string> Handle(BaseBeaconMessage response, string receiverId) => response switch
        {
            PermissionResponse permissionResponse => await HandlePermissionResponse(receiverId, permissionResponse),
            _ => _jsonSerializerService.Serialize(response)
        };

        private async Task<string> HandlePermissionResponse(string receiverId, PermissionResponse response)
        {
            var receiverAppMetadata = _appMetadataRepository.TryReadAsync(receiverId).Result;

            if (receiverAppMetadata == null)
                throw new Exception("AppMetadata not found");

            var permissionInfo = await _permissionInfoFactory.Create(
                receiverId,
                receiverAppMetadata,
                response.PublicKey,
                response.Network,
                response.Scopes);

            permissionInfo = _permissionInfoRepository.CreateOrUpdateAsync(permissionInfo).Result;
            OnPermissionsCreated?.Invoke(this, new ConnectedClientsListChangedEventArgs(receiverAppMetadata, permissionInfo));
            return _jsonSerializerService.Serialize(response);
        }
    }
}