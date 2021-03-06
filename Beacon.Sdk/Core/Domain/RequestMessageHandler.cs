namespace Beacon.Sdk.Core.Domain
{
    using System;
    using Beacon;
    using Beacon.Operation;
    using Beacon.Permission;
    using Interfaces;

    public class RequestMessageHandler
    {
        private readonly IAppMetadataRepository _appMetadataRepository;
        private readonly IJsonSerializerService _jsonSerializerService;

        public RequestMessageHandler(IAppMetadataRepository appMetadataRepository,
            IJsonSerializerService jsonSerializerService)
        {
            _appMetadataRepository = appMetadataRepository;
            _jsonSerializerService = jsonSerializerService;
        }

        public (AcknowledgeResponse, BaseBeaconMessage) Handle(string message, string senderId)
        {
            BaseBeaconMessage baseBeaconMessage = _jsonSerializerService.Deserialize<BaseBeaconMessage>(message);
            var ack = new AcknowledgeResponse(baseBeaconMessage.Id, senderId);

            return baseBeaconMessage.Type switch
            {
                BeaconMessageType.permission_request => (ack, HandlePermissionRequest(message)),
                BeaconMessageType.operation_request => (ack, HandleOperationRequest(message)),
                _ => throw new Exception("Unknown beaconMessage.Type.")
            };
        }

        private BaseBeaconMessage HandlePermissionRequest(string message)
        {
            PermissionRequest request = _jsonSerializerService.Deserialize<PermissionRequest>(message);

            _ = _appMetadataRepository.CreateOrUpdate(request.AppMetadata).Result;

            return request;
        }

        private BaseBeaconMessage HandleOperationRequest(string message)
        {
            OperationRequest request = _jsonSerializerService.Deserialize<OperationRequest>(message);

            _ = _appMetadataRepository.TryRead(request.SenderId).Result;

            return request;
        }
    }
}