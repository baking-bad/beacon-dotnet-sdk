namespace Beacon.Sdk.Core.Beacon
{
    public record BeaconBaseMessage
    {
        public string Type { get; init; }
        public string Version { get; init; } 
        public string Id { get; init; }
        public string SenderId { get; init; }
    };
}