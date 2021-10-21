namespace BeaconSdk.Infrastructure.Transport.P2P.ChannelOpening
{
    using MatrixSdk.Utils;

    /// <summary>
    /// Implements logic for building struct ChannelOpeningMessage.
    /// </summary>
    public interface IChannelOpeningMessageBuilder
    {
        void Reset();

        void BuildRecipientId(string relayServer, HexString publicKey);

        void BuildPairingPayload(string pairingRequestId, int payloadVersion, HexString senderPublicKeyHex,
            string senderRelayServer, string senderAppName);

        void BuildEncryptedPayload(HexString receiverPublicKeyHex);
        
        public ChannelOpeningMessage Message { get; }
    }
}