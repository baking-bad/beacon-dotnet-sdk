namespace Beacon.Sdk.Core.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Base58Check;
    using Beacon;
    using Beacon.Permission;
    using Interfaces;
    using Utils;

    public class OutgoingMessageHandler
    {
        private readonly IAppMetadataRepository _appMetadataRepository;
        private readonly IPermissionInfoRepository _permissionInfoRepository;
        private readonly ICryptographyService _cryptographyService;
        private readonly IJsonSerializerService _jsonSerializerService;

        public OutgoingMessageHandler(
            IAppMetadataRepository appMetadataRepository,
            IPermissionInfoRepository permissionInfoRepository,
            ICryptographyService cryptographyService,
            IJsonSerializerService jsonSerializerService)
        {
            _appMetadataRepository = appMetadataRepository;
            _permissionInfoRepository = permissionInfoRepository;
            _cryptographyService = cryptographyService;
            _jsonSerializerService = jsonSerializerService;
        }
        
        public string Handle(BeaconBaseMessage baseMessage, AppMetadata ownAppMetadata, string senderId, string receiverId)
        {
            if (baseMessage.Type == BeaconMessageType.acknowledge)
                return Handle((baseMessage as AcknowledgeResponse)!);
            if (baseMessage.Type == BeaconMessageType.permission_response)
                return Handle((baseMessage as PermissionResponse)!, ownAppMetadata, senderId, receiverId);
            
            throw new Exception("Invalid beacon message type");
        }

        private string Handle(AcknowledgeResponse response) =>_jsonSerializerService.Serialize(response);

        private string Handle(PermissionResponse intermediateResponse, AppMetadata ownAppMetadata, string senderId, string receiverId)
        {
            var response = new PermissionResponse(
                intermediateResponse.Id, 
                senderId, 
                ownAppMetadata, 
                intermediateResponse.Network, 
                intermediateResponse.Scopes, 
                intermediateResponse.PublicKey);

            AppMetadata receiverAppMetadata = _appMetadataRepository.TryRead(receiverId).Result ?? throw new Exception("AppMetadata not found");
            
            var permissionInfo = new PermissionInfo
            {
                AccountIdentifier = "",
                SenderId = senderId,
                AppMetadata = receiverAppMetadata,
                Website = "",
                Address = "",
                PublicKey = response.PublicKey,
                Network = response.Network,
                Scopes = response.Scopes,
                ConnectedAt = DateTimeOffset.Now
            };
            
            _ = _permissionInfoRepository.Create(permissionInfo).Result;
            
            return _jsonSerializerService.Serialize(response);
        }

        
        public string GetAddressFromPublicKey(string publicKey)
        {
            // tz1
            var edpkLength = 54;
            byte[] edpkPrefix = {6, 161, 159};
            
            // tz2
            var sppkLength = 55;
            byte[] sppkPrefix = {6, 161, 161};
            
            // tz3
            var p2pkLength = 55;
            byte[] p2pkPrefix = {6, 161, 164};

            byte[]? prefix = null;
            string? plainPublicKey = null;

            if (publicKey.Length == 64)
            {
                prefix = edpkPrefix;
                plainPublicKey = publicKey;
            }

            if (prefix == null || plainPublicKey == null)
                throw new Exception($"Invalid publicKey: {plainPublicKey}");

            if (!HexString.TryParse(plainPublicKey, out var plainHexPublicKey))
                throw new Invalid
            byte[] payload = _cryptographyService.Hash(plainHexPublicKey);

            byte[] b = prefix.Concat(payload).ToArray();
            return Base58CheckEncoding.Encode(b);
        }
        
        public string GetAccountIdentifier(string address, Network network)
        {
            var data = new List<string> {address, network.Type.ToString()};

            if (network.Name != null)
                data.Add($"name:{network.Name}");
            
            if (network.RpcUrl != null)
                data.Add($"rpc:{network.RpcUrl}");

            var m = string.Join('-', data);
            
            byte[] h =_cryptographyService.Hash(Encoding.UTF8.GetBytes(m), 10);
            
            return Base58CheckEncoding.Encode(h);
        }
    }
}