// ReSharper disable InconsistentNaming

namespace Beacon.Sdk.Beacon.Permission
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum NetworkType
    {
        mainnet,
        delphinet,
        edonet,
        florencenet,
        granadanet,
        hangzhounet,
        idiazabalnet,
        custom
    }
}