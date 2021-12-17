namespace Beacon.Sdk.Beacon
{
    using Core.Domain;

    public record AcknowledgeResponse : BaseBeaconMessage, IBeaconResponse
    {
        public AcknowledgeResponse(string id, string senderId) : base(BeaconMessageType.acknowledge, id, senderId)
        {
        }
    }
}