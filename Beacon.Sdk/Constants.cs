namespace Beacon.Sdk
{
    public static class Constants
    {
        public const string BeaconVersion = "2";

        public const string ConnectionString = "";
        
        public static readonly string[] KnownRelayServers =
        {
            // "beacon-node-1.sky.papers.tech",
            "beacon-node-0.papers.tech:8448",
            // "beacon-node-2.sky.papers.tech"
        }; // beacon-node-0.papers.tech:8448
        
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
        
        public static class NetworkType
        {
            public const string MAINNET = "mainnet";
            public const string DELPHINET = "delphinet";
            public const string EDONET = "edonet";
            public const string FLORENCENET = "florencenet";
            public const string GRANADANET = "granadanet";
            public const string HANGZHOUNET = "hangzhounet";
            public const string CUSTOM = "custom";
        }
        
        public static class PermissionScope
        {
            public const string SIGN = "sign"; // Allows the DApp to send requests to sign arbitrary payload
            public const string OPERATION_REQUEST = "operation_request"; // Allows the DApp to send requests to sign and broadcast Tezos Operations
            public const string ENCRYPT = "encrypt"; // Allows the DApp to send encryption and decryption requests
            public const string THRESHOLD = "threshold"; // Allows the DApp to sign transactions below a certain threshold. This is currently not fully defined and unused
        }
    }
}