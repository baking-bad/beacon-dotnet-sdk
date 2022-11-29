using Beacon.Sdk.Beacon.Sign;

namespace Beacon.Sdk.Core.Domain
{
    using System;
    using Beacon;
    using Beacon.Error;
    using Beacon.Operation;
    using Beacon.Permission;
    using Interfaces;
    using Newtonsoft.Json.Linq;

    public class DeserializeMessageHandler
    {
        private readonly IAppMetadataRepository _appMetadataRepository;
        private readonly IJsonSerializerService _jsonSerializerService;

        public DeserializeMessageHandler(IAppMetadataRepository appMetadataRepository,
            IJsonSerializerService jsonSerializerService)
        {
            _appMetadataRepository = appMetadataRepository;
            _jsonSerializerService = jsonSerializerService;
        }

        public (AcknowledgeResponse, BaseBeaconMessage) Handle(string message, string senderId)
        {
            var baseBeaconMessage = _jsonSerializerService.Deserialize<BaseBeaconMessage>(message);
            var ack = new AcknowledgeResponse(baseBeaconMessage.Id, senderId, baseBeaconMessage.Version);

            return (ack, baseBeaconMessage.Type switch
            {
                BeaconMessageType.permission_request => HandlePermissionRequest(message),
                BeaconMessageType.operation_request => HandleOperationRequest(message),
                BeaconMessageType.sign_payload_request =>
                    _jsonSerializerService.Deserialize<SignPayloadRequest>(message),
                BeaconMessageType.permission_response =>
                    _jsonSerializerService.Deserialize<PermissionResponse>(message),
                BeaconMessageType.operation_response =>
                    _jsonSerializerService.Deserialize<OperationResponse>(message),
                BeaconMessageType.sign_payload_response =>
                    _jsonSerializerService.Deserialize<SignPayloadResponse>(message),
                BeaconMessageType.acknowledge =>
                    _jsonSerializerService.Deserialize<AcknowledgeResponse>(message),
                BeaconMessageType.disconnect =>
                    _jsonSerializerService.Deserialize<DisconnectMessage>(message),
                BeaconMessageType.error =>
                    _jsonSerializerService.Deserialize<BaseBeaconError>(message),

                _ => throw new Exception("Unsupported Beacon message.")
            });
        }

        private BaseBeaconMessage HandleOperationRequest(string message)
        {
            var operationRequest = _jsonSerializerService.Deserialize<OperationRequest>(message);

            var nullEqParams = new JObject
            {
                ["entrypoint"] = "default",
                ["value"] = new JObject
                {
                    ["prim"] = "Unit"
                }
            };

            for (var i = 0; i < operationRequest.OperationDetails.Count; i++)
            {
                if (JToken.DeepEquals(operationRequest.OperationDetails[i].Parameters, nullEqParams))
                    operationRequest.OperationDetails[i] = new PartialTezosTransactionOperation(
                        operationRequest.OperationDetails[i].Amount,
                        operationRequest.OperationDetails[i].Destination,
                        null);
            }

            return operationRequest;
        }

        private BaseBeaconMessage HandlePermissionRequest(string message)
        {
            var permissionRequest = _jsonSerializerService.Deserialize<PermissionRequest>(message);
            _ = _appMetadataRepository.CreateOrUpdateAsync(permissionRequest.AppMetadata);
            return permissionRequest;
        }
    }
}