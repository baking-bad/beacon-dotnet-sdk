namespace Beacon.Sdk.Core.Beacon.Constants
{
    public static class BeaconMessageType
    {
        public const string PermissionRequest = "permission_request";

        public const string SignPayloadRequest = "sign_payload_request";

        // public const string // EncryptPayloadRequest = 'encrypt_payload_request';
        public const string OperationRequest = "operation_request";
        public const string BroadcastRequest = "broadcast_request";
        public const string PermissionResponse = "permission_response";

        public const string SignPayloadResponse = "sign_payload_response";

        // public const string // EncryptPayloadResponse = 'encrypt_payload_response',
        public const string OperationResponse = "operation_response";
        public const string BroadcastResponse = "broadcast_response";
        public const string Acknowledge = "acknowledge";
        public const string Disconnect = "disconnect";
        public const string Error = "error";
    }
}