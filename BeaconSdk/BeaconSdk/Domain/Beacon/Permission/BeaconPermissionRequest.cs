namespace BeaconSdk.Domain.Beacon.Permission
{
    using BaseMessage;

    public record BeaconPermissionRequest(
            string Version,
            string Id,
            string SenderId,
            AppMetadata AppMetadata,
            Network Network,
            BeaconPermissionScope[] Scopes
        )
        : BeaconBaseMessage(
            BeaconMessageType.permission_request,
            Version,
            Id,
            SenderId);


}