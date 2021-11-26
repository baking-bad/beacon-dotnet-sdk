namespace Beacon.Sdk.Beacon
{
    public record AcknowledgeResponse(string Version, string Id, string SenderId)
        : BeaconBaseMessage(BeaconMessageType.acknowledge, Version, Id, SenderId);
}