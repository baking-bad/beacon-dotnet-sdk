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

        public (AcknowledgeResponse, IBeaconRequest) Handle(string message, string senderId)
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

        private IBeaconRequest HandlePermissionRequest(string message)
        {
            PermissionRequest request = _jsonSerializerService.Deserialize<PermissionRequest>(message);

            _ = _appMetadataRepository.CreateOrUpdate(request.AppMetadata).Result;

            return request;
        }

        private IBeaconRequest HandleOperationRequest(string message)
        {
            OperationRequest request = _jsonSerializerService.Deserialize<OperationRequest>(message);

            _ = _appMetadataRepository.TryRead(request.SenderId).Result;

            return request;
        }
    }
}

// var options = new JsonSerializerOptions {MaxDepth = 100_000};

// var op = (Operation)DJson.Read($"{directory}/unsigned.json", options);
// dynamic? k = DJson.Parse(message, options);
// var d = (OperationContent) DJson.Parse(message, options);