namespace Beacon.Sdk.Core.Domain.P2P.ChannelOpening
{
    using Utils;

    /// <summary>
    ///     Implements logic for building struct ChannelOpeningMessage.
    /// </summary>
    public interface IChannelOpeningMessageBuilder
    {
        public ChannelOpeningMessage Message { get; }

        void Reset();

        void BuildRecipientId(string relayServer, HexString hexPublicKey);

        void BuildPairingPayload(string pairingRequestId, string payloadVersion,
            string senderRelayServer, string senderAppName, string? senderAppUrl, string? senderAppIcon);

        void BuildEncryptedPayload(HexString receiverHexPublicKey);
    }
}