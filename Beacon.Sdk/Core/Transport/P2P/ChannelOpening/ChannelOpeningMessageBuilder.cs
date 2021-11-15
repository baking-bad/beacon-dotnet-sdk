namespace Beacon.Sdk.Core.Transport.P2P.ChannelOpening
{
    using System;
    using Domain.Interfaces.Data;
    using Dto.Handshake;
    using Infrastructure.Serialization;
    using Matrix.Sdk.Core.Utils;
    using Sodium;

    public class ChannelOpeningMessageBuilder : IChannelOpeningMessageBuilder
    {
        private readonly IKeyPairRepository _keyPairRepository;
        private readonly ICryptographyService _cryptographyService;
        private readonly JsonSerializerService _jsonSerializerService;
        private ChannelOpeningMessage _message;

        public ChannelOpeningMessageBuilder(
            IKeyPairRepository keyPairRepository, 
            ICryptographyService cryptographyService,
            JsonSerializerService jsonSerializerService)
        {
            _cryptographyService = cryptographyService;
            _jsonSerializerService = jsonSerializerService;
            _keyPairRepository = keyPairRepository;
        }

        public ChannelOpeningMessage Message => _message;

        public void Reset() => _message = new ChannelOpeningMessage();

        public void BuildRecipientId(string relayServer, HexString hexPublicKey)
        {
            byte[] publicKeyByteArray = hexPublicKey.ToByteArray();
            byte[] hash = GenericHash.Hash(publicKeyByteArray, null, publicKeyByteArray.Length)!;

            if (!HexString.TryParse(hash, out HexString hexHash))
                throw new InvalidOperationException("Can not parse hash");

            _message.RecipientId = $"@{hexHash}:{relayServer}";
        }

        public void BuildPairingPayload(string pairingRequestId, int payloadVersion,
            string senderRelayServer, string senderAppName)
        {
            KeyPair keyPair = _keyPairRepository.KeyPair;
            
            if (!HexString.TryParse(keyPair!.PublicKey, out HexString senderHexPublicKey))
                throw new InvalidOperationException("Can not parse sender public key.");
            
            _message.Payload = payloadVersion switch
            {
                1 => senderHexPublicKey.ToString(),
                2 => BuildPairingPayloadV2(pairingRequestId, senderHexPublicKey, senderRelayServer, senderAppName),
                _ => throw new ArgumentOutOfRangeException(nameof(payloadVersion))
            };
        }

        public void BuildEncryptedPayload(HexString receiverHexPublicKey)
        {
            byte[] curve25519PublicKey =
                _cryptographyService.ConvertEd25519PublicKeyToCurve25519PublicKey(receiverHexPublicKey.ToByteArray());

            _message.Payload = _cryptographyService.EncryptMessageAsString(_message.Payload, curve25519PublicKey);
        }

        private string BuildPairingPayloadV2(string pairingRequestId, HexString senderHexPublicKey, string senderRelayServer, string senderAppName)
        {
            var pairingResponse = new P2PPairingResponse(
                pairingRequestId,
                senderAppName,
                senderHexPublicKey.Value,
                senderRelayServer);

            return _jsonSerializerService.Serialize(pairingResponse);
        }
    }
}