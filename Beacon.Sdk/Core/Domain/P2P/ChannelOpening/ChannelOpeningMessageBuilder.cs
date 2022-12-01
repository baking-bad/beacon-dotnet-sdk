using Beacon.Sdk.Core.Infrastructure.Cryptography;

namespace Beacon.Sdk.Core.Domain.P2P.ChannelOpening
{
    using System;
    using Dto.Handshake;
    using Interfaces;
    using Services;
    using Utils;

    public class ChannelOpeningMessageBuilder : IChannelOpeningMessageBuilder
    {
        private readonly ICryptographyService _cryptographyService;
        private readonly IJsonSerializerService _jsonSerializerService;
        private readonly KeyPairService _keyPairService;
        private ChannelOpeningMessage _message;

        public ChannelOpeningMessageBuilder(
            ICryptographyService cryptographyService,
            IJsonSerializerService jsonSerializerService,
            KeyPairService keyPairService)
        {
            _cryptographyService = cryptographyService;
            _jsonSerializerService = jsonSerializerService;
            _keyPairService = keyPairService;
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

        public void BuildPairingPayload(string pairingRequestId, string payloadVersion,
            string senderRelayServer, string senderAppName, string? senderAppUrl, string? senderAppIcon)
        {
            if (!HexString.TryParse(_keyPairService.KeyPair.PublicKey, out HexString senderHexPublicKey))
                throw new InvalidOperationException("Can not parse sender public key.");

            _message.Payload = payloadVersion switch
            {
                "1" => senderHexPublicKey.ToString(),
                "2" => BuildPairingResponseV2(pairingRequestId, senderHexPublicKey, senderRelayServer, senderAppName,
                    senderAppUrl, senderAppIcon),
                "3" => BuildPairingResponseV3(pairingRequestId, senderHexPublicKey, senderRelayServer, senderAppName,
                    senderAppUrl, senderAppIcon),
                _ => throw new ArgumentOutOfRangeException(nameof(payloadVersion))
            };
        }

        public void BuildEncryptedPayload(HexString receiverHexPublicKey)
        {
            byte[] curve25519PublicKey =
                _cryptographyService.ConvertEd25519PublicKeyToCurve25519PublicKey(receiverHexPublicKey.ToByteArray());

            string before = _message.Payload;
            _message.Payload = _cryptographyService.EncryptMessageAsString(before, curve25519PublicKey);
        }

        private string BuildPairingResponseV2(
            string pairingRequestId,
            HexString senderHexPublicKey,
            string senderRelayServer,
            string senderAppName,
            string? senderAppUrl = null,
            string? senderAppIcon = null)
        {
            var pairingResponse = new P2PPairingResponse(
                pairingRequestId,
                senderAppName,
                senderHexPublicKey.Value,
                senderRelayServer,
                Version: "2",
                AppUrl: senderAppUrl,
                Icon: senderAppIcon);

            return _jsonSerializerService.Serialize(pairingResponse);
        }

        private string BuildPairingResponseV3(
            string pairingRequestId,
            HexString senderHexPublicKey,
            string senderRelayServer,
            string senderAppName,
            string? senderAppUrl = null,
            string? senderAppIcon = null)
        {
            var pairingResponse = new P2PPairingResponse(
                pairingRequestId,
                senderAppName,
                senderHexPublicKey.Value,
                senderRelayServer,
                Version: "3",
                AppUrl: senderAppUrl,
                Icon: senderAppIcon);

            return _jsonSerializerService.Serialize(pairingResponse);
        }
    }
}