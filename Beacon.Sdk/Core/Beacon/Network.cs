namespace Beacon.Sdk.Core.Beacon
{
    public record Network
    {
        public string Type { get; init; }
        public string? Name { get; init; }
        public string? RpcUrl { get; init; }
    }
}