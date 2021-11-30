namespace Beacon.Sdk.Beacon
{
    using Core.Domain;

    public record AcknowledgeResponse(string Id, string SenderId)
        : BeaconBaseMessage(BeaconMessageType.acknowledge, Constants.MessageVersion, Id, SenderId), IBeaconResponse;
}