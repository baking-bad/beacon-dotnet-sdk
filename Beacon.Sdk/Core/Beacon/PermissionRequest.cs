namespace Beacon.Sdk.Core.Beacon
{
    using System.Collections.Generic;

    public record PermissionRequest(
            string Type,
            string Version,
            string Id,
            string SenderId,
            AppMetadata AppMetadata,
            Network Network,
            List<string> Scopes)
        : BeaconBaseMessage(
            Constants.BeaconMessageType.PermissionRequest,
            Version,
            Id,
            SenderId);
}