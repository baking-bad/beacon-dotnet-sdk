// ReSharper disable InconsistentNaming
namespace Beacon.Sdk.Beacon
{
    using System.Text.Json.Serialization;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum PermissionScope
    {
        sign,
        operation_request,
        encrypt,
        threshold
    }
}