using System.Threading.Tasks;
using Beacon.Sdk.Beacon.Sign;

namespace Beacon.Sdk.Core.Domain
{
    using System;
    using Beacon;
    using Beacon.Error;
    using Beacon.Operation;
    using Beacon.Permission;
    using Entities;
    using Interfaces;
    using Interfaces.Data;

    public class ResponseMessageHandler
    {
        private readonly IAppMetadataRepository _appMetadataRepository;
        private readonly IJsonSerializerService _jsonSerializerService;
        private readonly PermissionInfoFactory _permissionInfoFactory;
        private readonly IPermissionInfoRepository _permissionInfoRepository;

        public ResponseMessageHandler(
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

        public event EventHandler<DappConnectedEventArgs> OnDappConnected;

        public async Task<string> Handle(BaseBeaconMessage response, string receiverId) => response.Type switch
        {
            BeaconMessageType.acknowledge =>
                HandleAcknowledge(response as AcknowledgeResponse),
            BeaconMessageType.permission_response =>
                await HandlePermissionResponse(receiverId, response as PermissionResponse),
            BeaconMessageType.operation_response =>
                HandleOperationResponse(response as OperationResponse),
            BeaconMessageType.sign_payload_response =>
                HandleSignPayloadResponse(response as SignPayloadResponse),
            BeaconMessageType.disconnect =>
                HandleDisconnectResponse(response as DisconnectMessage),
            BeaconMessageType.error =>
                HandleError(response as BaseBeaconError),

            _ => throw new ArgumentException("Invalid beacon message type")
        };

        private string HandleAcknowledge(AcknowledgeResponse response) => _jsonSerializerService.Serialize(response);

        private async Task<string> HandlePermissionResponse(string receiverId, PermissionResponse response)
        {
            var receiverAppMetadata = _appMetadataRepository.TryReadAsync(receiverId).Result;

            if (receiverAppMetadata == null)
                throw new Exception("AppMetadata not found");

            var info = await _permissionInfoFactory.Create(
                receiverId,
                receiverAppMetadata,
                response.Address,
                response.PublicKey,
                response.Network,
                response.Scopes);

            info = _permissionInfoRepository.CreateOrUpdateAsync(info).Result;
            OnDappConnected?.Invoke(this, new DappConnectedEventArgs(receiverAppMetadata, info));
            return _jsonSerializerService.Serialize(response);
        }

        private string HandleOperationResponse(OperationResponse response) =>
            _jsonSerializerService.Serialize(response);

        private string HandleSignPayloadResponse(SignPayloadResponse response) =>
            _jsonSerializerService.Serialize(response);

        private string HandleDisconnectResponse(DisconnectMessage response) =>
            _jsonSerializerService.Serialize(response);

        private string HandleError(BaseBeaconError response)
        {
            if (response!.ErrorType == BeaconErrorType.ABORTED_ERROR)
            {
                BeaconAbortedError beaconAbortedError =
                    response as BeaconAbortedError ?? throw new InvalidOperationException();

                return _jsonSerializerService.Serialize(beaconAbortedError);
            }

            throw new ArgumentException("error.ErrorType");
        }
    }
}