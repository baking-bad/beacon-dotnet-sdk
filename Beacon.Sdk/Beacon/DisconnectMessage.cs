namespace Beacon.Sdk.Beacon
{
    public record DisconnectMessage : BaseBeaconMessage
    {
        public DisconnectMessage(string id, string senderId) : base(BeaconMessageType.disconnect, id, senderId)
        {
        }
    }
}