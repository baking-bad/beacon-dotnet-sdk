namespace Beacon.Sdk.Beacon.Permission
{
    // public record Network(NetworkType Type, string? Name, string? RpcUrl)
    // {
    //     public NetworkType Type { get; } = Type;
    //
    //     public string? Name { get; } = Name;
    //
    //     public string? RpcUrl { get; } = RpcUrl;
    // }

    // see: https://github.com/mbdavid/LiteDB/issues/1860
    public record Network
    {
        public NetworkType Type { get; set; }

        public string? Name { get; set; }

        public string? RpcUrl { get; set; }
    }
}