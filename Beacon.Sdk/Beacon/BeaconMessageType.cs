// ReSharper disable InconsistentNaming

namespace Beacon.Sdk.Beacon
{
    public enum BeaconMessageType
    {
        permission_request,
        sign_payload_request,
        operation_request,
        broadcast_request,
        permission_response,
        sign_payload_response,
        operation_response,
        broadcast_response,
        acknowledge,
        disconnect,
        error
    }
}