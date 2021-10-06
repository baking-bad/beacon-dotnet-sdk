// ReSharper disable InconsistentNaming

namespace BeaconSdk.Domain.Beacon
{
    public record Network(
        NetworkType Type,
        string? Name,
        string? RpcUrl
    );

    public enum NetworkType
    {
        mainnet,
        carthagenet,
        custom
    }
}