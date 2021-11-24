namespace Beacon.Sdk.Beacon
{
    public record BeaconBaseMessage(string Type, string Version, string Id, string SenderId);
}