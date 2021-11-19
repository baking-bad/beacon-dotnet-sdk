namespace Beacon.Sdk
{
    using System.Collections.Generic;

    public record BeaconBaseMessage
    {
        public string Type { get; init; }
        public string Version { get; init; } 
        public string Id { get; init; }
        public string SenderId { get; init; }
    };
    
    public record AppMetadata
    {
        public string SenderId { get; init; }
        public string Name { get; init; }
        public string? Icon { get; init; } // URL
    }
    public record PermissionRequest : BeaconBaseMessage
    {
        public AppMetadata AppMetadata { get; init; }
        public Network Network { get; init; }
        public List<string> Scopes { get; init; }
    }

    public record Network
    {
        public string Type { get; init; }
        public string? Name { get; init; }
        public string? RpcUrl { get; init; }
    }
}