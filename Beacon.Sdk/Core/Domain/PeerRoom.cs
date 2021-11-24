namespace Beacon.Sdk.Core.Domain
{
    using Utils;

    public class PeerRoom
    {
        public long Id { get; set; }

        public HexString PeerHexPublicKey { get; set; }

        public string RoomId { get; set; }
    }
}