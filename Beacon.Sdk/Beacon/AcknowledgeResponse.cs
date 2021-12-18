namespace Beacon.Sdk.Beacon
{
    public record AcknowledgeResponse : BaseBeaconMessage
    {
        public AcknowledgeResponse(string id, string senderId) : base(BeaconMessageType.acknowledge, id, senderId)
        {
        }
    }
}