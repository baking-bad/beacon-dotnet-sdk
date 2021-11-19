namespace Beacon.Sdk.Core.Beacon
{
    public record AcknowledgeResponse
    {
        public string Id { get; set; }
        public string Type => Constants.BeaconMessageType.Acknowledge;
    }
}