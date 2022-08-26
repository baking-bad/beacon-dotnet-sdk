namespace Beacon.Sdk.Core.Domain.Entities.P2P
{
    using Utils;

    public class P2PPeerRoom
    {
        public long Id { get; set; }
        public string P2PUserId { get; set; }
        public HexString PeerHexPublicKey { get; set; }
        public string PeerName { get; set; }
        public string RoomId { get; set; }
    }
}