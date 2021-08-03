namespace BeaconSdk.Domain.Beacon
{
    public record Network(NetworkType Type, string? Name, string? RpcUrl);

    public class NetworkType
    {
        public const string Mainnet = "mainnet";
        public const string Carthagenet = "carthagenet";
        public const string Custom = "custom";
    }
}