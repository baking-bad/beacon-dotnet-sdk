namespace Beacon.Sdk.Core.Infrastructure.Transport.P2P.ChannelOpening
{
    public struct ChannelOpeningMessage
    {
        public string RecipientId { get; set; }

        public string Payload { get; set; }

        public override string ToString() => $"@channel-open:{RecipientId}:{Payload}";
    }
}