namespace Beacon.Sdk.Core.Domain
{
    using Beacon;
    using Beacon.Permission;
    using Interfaces;
    
    public class IncomingMessageHandler
    {
        private readonly IAppMetadataRepository _appMetadataRepository;
        private readonly IJsonSerializerService _jsonSerializerService;
        
        public IncomingMessageHandler(IAppMetadataRepository appMetadataRepository,
            IJsonSerializerService jsonSerializerService)
        {
            _appMetadataRepository = appMetadataRepository;
            _jsonSerializerService = jsonSerializerService;
        }

        public BeaconBaseMessage Handle(BeaconBaseMessage baseMessage, string message)
        {
            if (baseMessage.Type == BeaconMessageType.PermissionRequest)
            {
                PermissionRequest permissionRequest = _jsonSerializerService.Deserialize<PermissionRequest>(message);
                
                return Handle(permissionRequest);
            }
            
            return baseMessage;
        }

        private PermissionRequest Handle(PermissionRequest permissionRequest)
        {
            AppMetadata appMetadata = _appMetadataRepository.CreateOrUpdate(permissionRequest.AppMetadata).Result;

            return permissionRequest;
        }
    }
}