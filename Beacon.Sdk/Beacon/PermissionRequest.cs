namespace Beacon.Sdk.Beacon
{
    using System.Collections.Generic;
    using Constants;

    public record PermissionRequest(
            string Type,
            string Version,
            string Id,
            string SenderId,
            AppMetadata AppMetadata,
            Network Network,
            List<string> Scopes)
        : BeaconBaseMessage(
            BeaconMessageType.PermissionRequest,
            Version,
            Id,
            SenderId);
}