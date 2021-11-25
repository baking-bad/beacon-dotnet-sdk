namespace Beacon.Sdk.Core.Domain
{
    using Utils;

    public class Peer
    {
        public long Id { get; set; }

        public string SenderUserId { get; set; }

        public HexString HexPublicKey { get; set; }

        public string Name { get; set; }

        public string Version { get; set; }

        public string RelayServer { get; set; }
    }
}