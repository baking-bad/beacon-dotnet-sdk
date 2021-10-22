namespace Beacon.Sdk.Core.Infrastructure.Transport.P2P.ChannelOpening
{
    using System;
    using System.Text;
    using Domain.Pairing;
    using Matrix.Sdk.Core.Utils;
    using Serialization;
    using Sodium;

    public class ChannelOpeningMessageBuilder : IChannelOpeningMessageBuilder
    {
        private readonly JsonSerializerService _jsonSerializerService;
        private ChannelOpeningMessage _message;

        public ChannelOpeningMessageBuilder(JsonSerializerService jsonSerializerService)
        {
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
            byte[]? result =
                PublicKeyAuth.ConvertEd25519PublicKeyToCurve25519PublicKey(receiverPublicKeyHex.ToByteArray());
            byte[]? payload = SealedPublicKeyBox.Create(_message.Payload, result);

            _message.Payload = Encoding.UTF8.GetString(payload);
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