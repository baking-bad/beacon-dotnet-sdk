namespace Beacon.Sdk.Beacon
{
    using Core.Domain;

    // public record AcknowledgeResponse(string Id, string SenderId)
    //     : BeaconBaseMessage(BeaconMessageType.acknowledge, Constants.MessageVersion, Id, SenderId), IBeaconResponse;


    public class AcknowledgeResponse : BaseBeaconMessage, IBeaconResponse
    {
        public AcknowledgeResponse(string id, string senderId) : base(BeaconMessageType.acknowledge, id)
        {
            SenderId = senderId;
        }
    }
}