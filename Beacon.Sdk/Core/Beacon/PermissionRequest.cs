namespace Beacon.Sdk.Core.Beacon
{
    using System.Collections.Generic;

    public record PermissionRequest : BeaconBaseMessage
    {
        public AppMetadata AppMetadata { get; init; }
        public Network Network { get; init; }
        public List<string> Scopes { get; init; }
    }
}