namespace Beacon.Sdk.Core.Domain
{
    using System;
    using Beacon;
    using Beacon.Permission;
    using Entities;
    using Interfaces;

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
                _ => throw new Exception("Invalid beacon message type")
            };

        private string HandleAcknowledge(IBeaconResponse response)
        {
            var ack = response as AcknowledgeBeaconResponse;

            return _jsonSerializerService.Serialize(ack!);
        }

        private string HandlePermissionResponse(IBeaconResponse beaconResponse, AppMetadata ownAppMetadata, string senderId,
            string receiverId)
        {
            var response = beaconResponse as PermissionResponse;
            var newResponse = new PermissionResponse(
                response!.Id,
                senderId,
                ownAppMetadata,
                response.Network,
                response.Scopes,
                response.PublicKey);

            AppMetadata receiverAppMetadata = _appMetadataRepository.TryRead(receiverId).Result ??
                                              throw new Exception("AppMetadata not found");

            PermissionInfo info = _permissionInfoFactory.Create(senderId, receiverAppMetadata, newResponse.PublicKey,
                newResponse.Network, newResponse.Scopes);

            info = _permissionInfoRepository.Create(info).Result;

            return _jsonSerializerService.Serialize(newResponse);
        }
    }
}