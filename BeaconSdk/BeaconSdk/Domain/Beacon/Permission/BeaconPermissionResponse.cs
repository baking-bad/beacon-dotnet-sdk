namespace BeaconSdk.Domain.Beacon.Permission
{
    using BaseMessage;

    public record BeaconPermissionResponse(
            string Version,
            string Id,
            string SenderId,
            string PublicKey,
            Network Network,
            BeaconPermissionScope[] Scopes,
            Threshold Threshold
        )
        : BeaconBaseMessage(
            BeaconMessageType.permission_response,
            Version,
            Id,
            SenderId);
}