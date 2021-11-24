namespace Beacon.Sdk.Core.Domain
{
    using Utils;

    public class BeaconPeerRoom
    {
        public long Id { get; set; }

        public HexString BeaconPeerHexPublicKey { get; set; }

        public string RoomId { get; set; }
    }
}