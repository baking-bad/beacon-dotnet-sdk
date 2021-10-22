namespace Beacon.Sdk.Core.Domain.Beacon.P2P
{
    using System;

    public enum BeaconConnectionKind
    {
        P2P
    }

    public record BeaconPeer(string? Id, string Name, string PublicKey, string RelayServer, string Version,
        string? Icon, Uri? AppUri);
}