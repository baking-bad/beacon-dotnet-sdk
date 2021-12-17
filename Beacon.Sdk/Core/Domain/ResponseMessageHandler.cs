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

        public string Handle(IBeaconResponse response, AppMetadata ownAppMetadata, string senderId,
            string receiverId) =>
            response.Type switch
            {
                BeaconMessageType.acknowledge => HandleAcknowledge(response),
                BeaconMessageType.permission_response => HandlePermissionResponse(response, ownAppMetadata, senderId,
                    receiverId),
                BeaconMessageType.operation_response => HandleOperationResponse(response, senderId),
                BeaconMessageType.error => HandleError(response, senderId),
                _ => throw new Exception("Invalid beacon message type")
            };

        private string HandleAcknowledge(IBeaconResponse response)
        {
            var ack = response as AcknowledgeResponse;

            return _jsonSerializerService.Serialize(ack!);
        }

        private string HandlePermissionResponse(IBeaconResponse beaconResponse, AppMetadata ownAppMetadata,
            string senderId,
            string receiverId)
        {
            var response = beaconResponse as PermissionResponse;
            response!.SenderId = senderId;
            response.AppMetadata = ownAppMetadata;

            AppMetadata receiverAppMetadata = _appMetadataRepository.TryRead(receiverId).Result ??
                                              throw new Exception("AppMetadata not found");

            PermissionInfo info = _permissionInfoFactory.Create(receiverId, receiverAppMetadata,
                PubKey.FromBase58(response.PublicKey),
                response.Network, response.Scopes);

            info = _permissionInfoRepository.Create(info).Result;

            return _jsonSerializerService.Serialize(response);
        }

        private string HandleOperationResponse(IBeaconResponse beaconResponse, string senderId)
        {
            var response = beaconResponse as OperationResponse;
            response!.SenderId = senderId;

            return _jsonSerializerService.Serialize(response);
        }

        private string HandleError(IBeaconResponse response, string senderId)
        {
            var error = response as BaseBeaconError;

            if (error!.ErrorType == BeaconErrorType.ABORTED_ERROR)
            {
                var abortedError = response as BeaconAbortedError;
                abortedError!.SenderId = senderId;

                return _jsonSerializerService.Serialize(abortedError);
            }

            throw new ArgumentException("error.ErrorType");
        }
    }
}