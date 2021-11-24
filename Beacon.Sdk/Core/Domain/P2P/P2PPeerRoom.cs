namespace Beacon.Sdk.Core.Domain.P2P
{
    using Utils;

    public class P2PPeerRoom
    {
        public long Id { get; set; }

        public string P2PUserId { get; set; }
        
        public HexString PeerHexPublicKey { get; set; }

        public string RoomId { get; set; }
    }
}