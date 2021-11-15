namespace Beacon.Sdk.Core.Transport.P2P.ChannelOpening
{
    using Matrix.Sdk.Core.Utils;

    /// <summary>
    ///     Implements logic for building struct ChannelOpeningMessage.
    /// </summary>
    public interface IChannelOpeningMessageBuilder
    {
        public ChannelOpeningMessage Message { get; }

        void Reset();

        void BuildRecipientId(string relayServer, HexString hexPublicKey);

        void BuildPairingPayload(string pairingRequestId, int payloadVersion,
            string senderRelayServer, string senderAppName);

        void BuildEncryptedPayload(HexString receiverHexPublicKey);
    }
}