namespace Beacon.Sdk.Core.Domain
{
    using Beacon;
    using Beacon.Constants;

    public class OutgoingMessageHandler
    {
        private readonly IAppMetadataRepository _appMetadataRepository;

        public OutgoingMessageHandler(IAppMetadataRepository appMetadataRepository)
        {
            _appMetadataRepository = appMetadataRepository;
        }
        
        public BeaconBaseMessage Handle(BeaconBaseMessage baseMessage)
        {
            if (baseMessage.Type == BeaconMessageType.PermissionRequest)
                return Handle((baseMessage as PermissionRequest)!);
            
            return baseMessage;
        }

        private PermissionRequest Handle(PermissionRequest permissionRequest)
        {
            _ = _appMetadataRepository.CreateOrUpdate(permissionRequest.AppMetadata).Result;

            return permissionRequest;
        }
    }
}