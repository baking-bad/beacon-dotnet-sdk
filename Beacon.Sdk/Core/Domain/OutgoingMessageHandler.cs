namespace Beacon.Sdk.Core.Domain
{
    using System;
    using Beacon;
    using Beacon.Permission;
    using Interfaces;

    public class OutgoingMessageHandler
    {
        private readonly IAppMetadataRepository _appMetadataRepository;
        private readonly IJsonSerializerService _jsonSerializerService;

        public OutgoingMessageHandler(IAppMetadataRepository appMetadataRepository, IJsonSerializerService jsonSerializerService)
        {
            _appMetadataRepository = appMetadataRepository;
            _jsonSerializerService = jsonSerializerService;
        }
        
        public string Handle(BeaconBaseMessage baseMessage)
        {
            if (baseMessage.Type == BeaconMessageType.PermissionResponse)
                return Handle((baseMessage as PermissionResponse)!);

            throw new Exception("Invalid beacon message type");
        }

        private string Handle(PermissionResponse permissionResponse, AppMetadata ownAppMetadata)
        {
            AppMetadata appMetadata = _appMetadataRepository.TryRead(permissionResponse.SenderId).Result ?? throw new Exception("AppMetadata not found");
            
            _ = _appMetadataRepository.CreateOrUpdate(permissionResponse.AppMetadata).Result;

            permissionResponse.AppMetadata = ownAppMetadata;
            // permissionResponse.Network = new Network(String.Empty, ErrorType.Aborted);
            // var newPermissionResponse = new PermissionResponse()
            return _jsonSerializerService.Serialize(permissionResponse);
        }
    }
}