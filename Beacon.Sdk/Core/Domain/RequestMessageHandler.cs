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
            BeaconBaseMessage beaconMessage = _jsonSerializerService.Deserialize<BeaconBaseMessage>(message);
            var ack = new AcknowledgeResponse(beaconMessage.Id, senderId);

            return beaconMessage.Type switch
            {
                BeaconMessageType.permission_request => (ack, HandlePermissionRequest(message)),
                BeaconMessageType.operation_request => (ack, HandleOperationRequest(message)),
                _ => (ack, beaconMessage)
            };
        }

        public IBeaconRequest HandlePermissionRequest(string message)
        {
            PermissionRequest request = _jsonSerializerService.Deserialize<PermissionRequest>(message);

            _ = _appMetadataRepository.CreateOrUpdate(request.AppMetadata).Result;

            return request;
        }

        public IBeaconRequest HandleOperationRequest(string message)
        {
            try
            {
                OperationRequest request = _jsonSerializerService.Deserialize<OperationRequest>(message);

                // _appMetadataRepository.TryRead(beaconMessage.SenderId).Result;

                return request;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            // OperationRequest request = _jsonSerializerService.Deserialize<OperationRequest>(message);
            //
            // // _appMetadataRepository.TryRead(beaconMessage.SenderId).Result;
            //
            // return request;
        }
    }
}