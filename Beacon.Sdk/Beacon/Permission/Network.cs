namespace Beacon.Sdk.Beacon.Permission
{
    public record Network
    {
        public Network(NetworkType type, string? name, string? rpcUrl)
        {
            Type = type;
            Name = name;
            RpcUrl = rpcUrl;
        }

        public NetworkType Type { get; set; }

        public string? Name { get; set; }

        public string? RpcUrl { get; set; }
    }
}