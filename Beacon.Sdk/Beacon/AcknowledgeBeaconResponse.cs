namespace Beacon.Sdk.Beacon
{
    using Core.Domain;

    public record AcknowledgeBeaconResponse(string Id, string SenderId)
        : BeaconBaseMessage(BeaconMessageType.acknowledge, Constants.MessageVersion, Id, SenderId), IBeaconResponse;

    // public class AcknowledgeResponse : BeaconBaseMessage
    // {
    //     public AcknowledgeResponse(string id, string senderId) : base(BeaconMessageType.acknowledge, id, senderId)
    //     {
    //     }
    // }
}