namespace BeaconSdk.Domain.Beacon
{
    using System;

    public enum BeaconConnectionKind
    {
        p2p
    }

    public record BeaconPeer(string Id, string Name, string PublicKey, string RelayServer, string Version, string? Icon, Uri? AppUri);

}