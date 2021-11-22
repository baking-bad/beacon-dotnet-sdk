namespace Beacon.Sdk.Core.Beacon
{
    public record AcknowledgeResponse(string Version, string Id, string SenderId)
        : BeaconBaseMessage(Constants.BeaconMessageType.Acknowledge, Version, Id, SenderId);
}