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
    using Netezos.Keys;

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

        public string Handle(BaseBeaconMessage response, string receiverId) =>
            response.Type switch
            {
                BeaconMessageType.acknowledge => HandleAcknowledge(response as AcknowledgeResponse),
                BeaconMessageType.permission_response => HandlePermissionResponse(receiverId,
                    response as PermissionResponse),
                BeaconMessageType.operation_response => HandleOperationResponse(response as OperationResponse),
                BeaconMessageType.error => HandleError(response as BaseBeaconError),

                _ => throw new ArgumentException("Invalid beacon message type")
            };

        private string HandleAcknowledge(AcknowledgeResponse response) => _jsonSerializerService.Serialize(response);

        private string HandlePermissionResponse(string receiverId, PermissionResponse response)
        {
            AppMetadata? receiverAppMetadata = _appMetadataRepository.TryRead(receiverId).Result;

            if (receiverAppMetadata == null)
                throw new Exception("AppMetadata not found");

            // PermissionInfo info = _permissionInfoFactory.Create(
            //     receiverId,
            //     receiverAppMetadata,
            //     PubKey.FromBase58(response.PublicKey),
            //     response.Network,
            //     response.Scopes);
            //
            // info = _permissionInfoRepository.Create(info).Result;

            return _jsonSerializerService.Serialize(response);
        }

        private string HandleOperationResponse(OperationResponse response) =>
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