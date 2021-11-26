namespace Beacon.Sdk.Beacon.Constants
{
    public static class PermissionScopeJson
    {
        public const string SIGN = "sign"; // Allows the DApp to send requests to sign arbitrary payload

        public const string
            OPERATION_REQUEST =
                "operation_request"; // Allows the DApp to send requests to sign and broadcast Tezos Operations

        public const string ENCRYPT = "encrypt"; // Allows the DApp to send encryption and decryption requests

        public const string
            THRESHOLD =
                "threshold"; // Allows the DApp to sign transactions below a certain threshold. This is currently not fully defined and unused
    }
}