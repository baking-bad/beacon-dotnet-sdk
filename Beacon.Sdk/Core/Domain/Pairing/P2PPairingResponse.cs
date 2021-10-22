namespace Beacon.Sdk.Core.Domain.Pairing
{
    public record P2PPairingResponse(string Id,
        string Name,
        string PublicKey,
        string RelayServer,
        string Type = "p2p-pairing-response",
        string Version = Constants.BeaconVersion);
}