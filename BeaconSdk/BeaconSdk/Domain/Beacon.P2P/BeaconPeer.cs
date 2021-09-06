namespace BeaconSdk.Domain.Beacon.P2P
{
    using System;

    public enum BeaconConnectionKind
    {
        p2p
    }

    public record BeaconPeer(string? Id, string Name, string PublicKey, string RelayServer, string Version, string? Icon, Uri? AppUri);

}