namespace Beacon.Sdk.Core.Domain
{
    using System;
    using System.Text.Json;
    using Beacon;
    using Beacon.Operation;
    using Beacon.Permission;
    using Dynamic.Json;
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
                var options = new JsonSerializerOptions {MaxDepth = 100_000};

                // var op = (Operation)DJson.Read($"{directory}/unsigned.json", options);
                dynamic? k = DJson.Parse(message, options);
                // var d = (OperationContent) DJson.Parse(message, options);
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