namespace Beacon.Sdk.Core.Domain.Entities
{
    using Utils;

    public class Peer
    {
        public long Id { get; set; }
        public string SenderId { get; set; }
        public HexString HexPublicKey { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
        public string RelayServer { get; set; }
        public string ConnectedAddress { get; set; }
    }
}