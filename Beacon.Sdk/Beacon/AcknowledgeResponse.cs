namespace Beacon.Sdk.Beacon
{
    public record AcknowledgeResponse(string Id, string SenderId) 
        : BeaconBaseMessage(BeaconMessageType.acknowledge, Constants.MessageVersion, Id, SenderId);

    // public class AcknowledgeResponse : BeaconBaseMessage
    // {
    //     public AcknowledgeResponse(string id, string senderId) : base(BeaconMessageType.acknowledge, id, senderId)
    //     {
    //     }
    // }
}