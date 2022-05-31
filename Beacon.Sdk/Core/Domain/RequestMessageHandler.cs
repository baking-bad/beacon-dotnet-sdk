using Beacon.Sdk.Beacon.Sign;

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
            var baseBeaconMessage = _jsonSerializerService.Deserialize<BaseBeaconMessage>(message);
            var ack = new AcknowledgeResponse(baseBeaconMessage.Id, senderId, baseBeaconMessage.Version);

            return baseBeaconMessage.Type switch
            {
                BeaconMessageType.permission_request => (ack, HandlePermissionRequest(message)),
                BeaconMessageType.operation_request => (ack, HandleOperationRequest(message)),
                BeaconMessageType.sign_payload_request => (ack, HandleSignPayloadRequest(message)),
                _ => throw new Exception("Unknown beaconMessage.Type.")
            };
        }

        private BaseBeaconMessage HandlePermissionRequest(string message)
        {
            var request = _jsonSerializerService.Deserialize<PermissionRequest>(message);
            _ = _appMetadataRepository.CreateOrUpdateAsync(request.AppMetadata).Result;

            return request;
        }

        private BaseBeaconMessage HandleOperationRequest(string message)
        {
            var operationRequest = _jsonSerializerService.Deserialize<OperationRequest>(message);
            _ = _appMetadataRepository.TryReadAsync(operationRequest.SenderId).Result;

            return operationRequest;
        }


        private BaseBeaconMessage HandleSignPayloadRequest(string message)
        {
            var signPayloadRequest = _jsonSerializerService.Deserialize<SignPayloadRequest>(message);
            return signPayloadRequest;
        }
    }
}