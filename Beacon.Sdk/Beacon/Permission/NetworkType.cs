// ReSharper disable InconsistentNaming

namespace Beacon.Sdk.Beacon.Permission
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum NetworkType
    {
        mainnet,
        ghostnet, // Long running testnet
        mondaynet, // Testnet, resets every monday
        dailynet, // Testnet, resets every day
        delphinet,
        edonet,
        florencenet,
        granadanet,
        hangzhounet,
        ithacanet,
        jakartanet,
        kathmandunet,
        custom
    }
}