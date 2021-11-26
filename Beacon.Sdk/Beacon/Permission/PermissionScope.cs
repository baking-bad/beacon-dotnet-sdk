// ReSharper disable InconsistentNaming
namespace Beacon.Sdk.Beacon.Permission
{
    using System.Text.Json.Serialization;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum PermissionScope
    {
        sign = 0, // Allows the DApp to send requests to sign arbitrary payload
        operation_request = 1, // Allows the DApp to send requests to sign and broadcast Tezos Operations
        encrypt = 2, // Allows the DApp to send encryption and decryption requests
        threshold = 3 // Allows the DApp to sign transactions below a certain threshold. This is currently not fully defined and unused
    }
}