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
            BaseBeaconMessage baseBeaconMessage = _jsonSerializerService.Deserialize<BaseBeaconMessage>(message);
            var ack = new AcknowledgeResponse(baseBeaconMessage.Id, senderId, baseBeaconMessage.Version);

            return baseBeaconMessage.Type switch
            {
                BeaconMessageType.permission_request => (ack, HandlePermissionRequest(message)),
                BeaconMessageType.operation_request => (ack, HandleOperationRequest(message)),
                _ => throw new Exception("Unknown beaconMessage.Type.")
            };
        }

        private BaseBeaconMessage HandlePermissionRequest(string message)
        {
            PermissionRequest request = _jsonSerializerService.Deserialize<PermissionRequest>(message);

            _ = _appMetadataRepository.CreateOrUpdateAsync(request.AppMetadata).Result;

            return request;
        }

        private BaseBeaconMessage HandleOperationRequest(string message)
        {
            // todo: Deserialize this
            // {
            //     "id": "39250d96-050d-3d0c-8fe8-369545d82a40",
            //     "version": "2",
            //     "senderId": "2Z5r4gMaaPvwc",
            //     "type": "operation_request",
            //     "network": {
            //         "type": "mainnet",
            //         "name": "Mainnet",
            //         "rpcUrl": "https://rpc.tzkt.io/mainnet"
            //     },
            //     "operationDetails": [{
            //         "kind": "transaction",
            //         "amount": "1000000",
            //         "destination": "KT1D6XTy8oAHkUWdzuQrzySECCDMnANEchQq",
            //         "parameters": {
            //             "entrypoint": "bet",
            //             "value": {
            //                 "prim": "Pair",
            //                 "args": [{
            //                     "prim": "Pair",
            //                     "args": [{
            //                         "prim": "Left",
            //                         "args": [{
            //                             "prim": "Unit"
            //                         }]
            //                     }, {
            //                         "int": "1515"
            //                     }]
            //                 }, {
            //                     "int": "1933606"
            //                 }]
            //             }
            //         }
            //     }],
            //     "sourceAddress": "tz1RDE4JdUo73bx23cwjr97gDkcP63b4NfGD"
            // }
            // Newtonsoft.Json.JsonReaderException: Unexpected character encountered while parsing value: {. Path 'operationDetails[0].parameters', line 1, position 326.
            //         at Newtonsoft.Json.JsonTextReader.ReadStringValue(ReadType readType)
            //     at Newtonsoft.Json.Json…
            
            OperationRequest request = _jsonSerializerService.Deserialize<OperationRequest>(message);

            _ = _appMetadataRepository.TryReadAsync(request.SenderId).Result;

            return request;
        }
    }
}