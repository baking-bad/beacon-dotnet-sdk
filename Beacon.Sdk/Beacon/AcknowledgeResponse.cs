namespace Beacon.Sdk.Beacon
{
    public record AcknowledgeResponse : BaseBeaconMessage
    {
        public AcknowledgeResponse(string id, string senderId, string version) :
            base(BeaconMessageType.acknowledge, version, id, senderId)
        {
        }
    }
}