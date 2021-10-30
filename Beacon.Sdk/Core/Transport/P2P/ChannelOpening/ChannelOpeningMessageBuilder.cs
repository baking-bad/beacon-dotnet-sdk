namespace Beacon.Sdk.Core.Transport.P2P.ChannelOpening
{
    using System;
    using Dto.Handshake;
    using Infrastructure.Serialization;
    using Matrix.Sdk.Core.Utils;
    using Sodium;

    public class ChannelOpeningMessageBuilder : IChannelOpeningMessageBuilder
    {
        private readonly ICryptographyService _cryptographyService;
        private readonly JsonSerializerService _jsonSerializerService;
        private ChannelOpeningMessage _message;


        public ChannelOpeningMessageBuilder(ICryptographyService cryptographyService,
            JsonSerializerService jsonSerializerService)
        {
            _cryptographyService = cryptographyService;
            _jsonSerializerService = jsonSerializerService;
        }

        public ChannelOpeningMessage Message => _message;

        public void Reset()
        {
            _message = new ChannelOpeningMessage();
        }

        public void BuildRecipientId(string relayServer, HexString publicKey)
        {
            byte[] publicKeyByteArray = publicKey.ToByteArray();
            byte[] hash = GenericHash.Hash(publicKeyByteArray, null, publicKeyByteArray.Length)!;

            if (!HexString.TryParse(hash, out HexString hexHash))
                throw new InvalidOperationException("Can not parse hash");

            _message.RecipientId = $"@{hexHash}:{relayServer}";
        }

        public void BuildPairingPayload(string pairingRequestId, int payloadVersion, HexString senderPublicKeyHex,
            string senderRelayServer, string senderAppName)
        {
            _message.Payload = payloadVersion switch
            {
                1 => senderPublicKeyHex.ToString(),
                2 => BuildPairingPayloadV2(pairingRequestId, senderPublicKeyHex, senderRelayServer, senderAppName),
                _ => throw new ArgumentOutOfRangeException(nameof(payloadVersion))
            };
        }

        public void BuildEncryptedPayload(HexString receiverPublicKeyHex)
        {
            byte[] curve25519PublicKey =
                _cryptographyService.ConvertEd25519PublicKeyToCurve25519PublicKey(receiverPublicKeyHex.ToByteArray());

            _message.Payload = _cryptographyService.EncryptMessageAsString(_message.Payload, curve25519PublicKey);
        }

        private string BuildPairingPayloadV2(string pairingRequestId, HexString senderPublicKeyHex,
            string senderRelayServer,
            string senderAppName)
        {
            var pairingResponse = new P2PPairingResponse(
                pairingRequestId,
                senderAppName,
                senderPublicKeyHex.Value,
                senderRelayServer);

            return _jsonSerializerService.Serialize(pairingResponse);
        }
    }
}