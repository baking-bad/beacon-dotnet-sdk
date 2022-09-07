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
            switch (response.ErrorType)
            {
                case BeaconErrorType.ABORTED_ERROR:
                    return _jsonSerializerService
                        .Serialize(response as BeaconAbortedError ?? throw new InvalidOperationException());

                case BeaconErrorType.UNKNOWN_ERROR:
                    return _jsonSerializerService
                        .Serialize(response as UnknownBeaconError ?? throw new InvalidOperationException());

                case BeaconErrorType.TRANSACTION_INVALID_ERROR:
                    return _jsonSerializerService
                        .Serialize(response as TransactionInvalidBeaconError ?? throw new InvalidOperationException());

                case BeaconErrorType.BROADCAST_ERROR:
                    return _jsonSerializerService
                        .Serialize(response as BroadcastBeaconError ?? throw new InvalidOperationException());

                case BeaconErrorType.NETWORK_NOT_SUPPORTED_ERROR:
                    return _jsonSerializerService
                        .Serialize(response as NetworkNotSupportedBeaconError ?? throw new InvalidOperationException());

                case BeaconErrorType.NO_ADDRESS_ERROR:
                    return _jsonSerializerService
                        .Serialize(response as NoAddressBeaconError ?? throw new InvalidOperationException());

                case BeaconErrorType.NO_PRIVATE_KEY_FOUND_ERROR:
                    return _jsonSerializerService
                        .Serialize(response as NoPrivateKeyBeaconError ?? throw new InvalidOperationException());

                case BeaconErrorType.NOT_GRANTED_ERROR:
                    return _jsonSerializerService
                        .Serialize(response as NotGrantedBeaconError ?? throw new InvalidOperationException());

                case BeaconErrorType.PARAMETERS_INVALID_ERROR:
                    return _jsonSerializerService
                        .Serialize(response as ParametersInvalidBeaconError ?? throw new InvalidOperationException());

                case BeaconErrorType.TOO_MANY_OPERATIONS_ERROR:
                    return _jsonSerializerService
                        .Serialize(response as TooManyOperationsBeaconError ?? throw new InvalidOperationException());

                case BeaconErrorType.SIGNATURE_TYPE_NOT_SUPPORTED:
                    return _jsonSerializerService
                        .Serialize(response as SignatureTypeNotSupportedBeaconError ??
                                   throw new InvalidOperationException());

                default:
                    throw new ArgumentException("error.ErrorType");
            }
        }
    }
}