// ReSharper disable InconsistentNaming

namespace Beacon.Sdk.Beacon.Permission
{
    using System.Text.Json.Serialization;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum PermissionScope
    {
        sign, // Allows the DApp to send requests to sign arbitrary payload
        operation_request, // Allows the DApp to send requests to sign and broadcast Tezos Operations
        encrypt, // Allows the DApp to send encryption and decryption requests
        threshold // Allows the DApp to sign transactions below a certain threshold. This is currently not fully defined and unused
    }
}