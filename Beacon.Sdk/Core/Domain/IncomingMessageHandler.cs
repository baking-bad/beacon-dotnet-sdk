namespace Beacon.Sdk.Core.Domain
{
    using Beacon;
    using Beacon.Operation;
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

        public (AcknowledgeResponse, BeaconBaseMessage) Handle(string message, string senderId)
        {
            BeaconBaseMessage beaconMessage = _jsonSerializerService.Deserialize<BeaconBaseMessage>(message);
            var ack = new AcknowledgeResponse(beaconMessage.Id, senderId);

            if (beaconMessage.Type == BeaconMessageType.permission_request)
            {
                PermissionRequest permissionRequest = _jsonSerializerService.Deserialize<PermissionRequest>(message);

                _ = _appMetadataRepository.CreateOrUpdate(permissionRequest.AppMetadata).Result;
                
                return (ack, permissionRequest);
            }
            else if (beaconMessage.Type == BeaconMessageType.operation_request)
            {
                OperationRequest operationRequest = _jsonSerializerService.Deserialize<OperationRequest>(message);
                // _appMetadataRepository.TryRead(beaconMessage.SenderId).Result;
                
                return (ack, operationRequest);
            }
            else if (beaconMessage.Type == BeaconMessageType.sign_payload_request)
            {
                
            }
            else
            {
                
            }
            
            return (ack, beaconMessage);
        }
    }
}