namespace Beacon.Sdk.Beacon
{
    using Constants;

    public record AcknowledgeResponse(string Version, string Id, string SenderId)
        : BeaconBaseMessage(BeaconMessageType.Acknowledge, Version, Id, SenderId);
}