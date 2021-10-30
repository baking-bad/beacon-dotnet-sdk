// ReSharper disable InconsistentNaming

namespace Beacon.Sdk.Core
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