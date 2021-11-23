namespace Beacon.Sdk.Core.Beacon
{
    using Constants;

    public record AcknowledgeResponse(string Version, string Id, string SenderId)
        : BeaconBaseMessage(BeaconMessageType.Acknowledge, Version, Id, SenderId);
}