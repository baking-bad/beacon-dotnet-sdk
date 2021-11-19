namespace Beacon.Sdk.Core.Beacon
{
    public record AppMetadata
    {
        public string SenderId { get; init; }
        public string Name { get; init; }
        public string? Icon { get; init; } // URL
    }
}