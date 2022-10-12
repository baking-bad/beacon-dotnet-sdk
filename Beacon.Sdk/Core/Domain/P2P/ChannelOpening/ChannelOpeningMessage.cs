namespace Beacon.Sdk.Core.Domain.P2P.ChannelOpening
{
    public struct ChannelOpeningMessage
    {
        public const string StartPrefix = "@channel-open";
        public string RecipientId { get; set; }

        public string Payload { get; set; }

        public override string ToString() => $"{StartPrefix}:{RecipientId}:{Payload}";
    }
}