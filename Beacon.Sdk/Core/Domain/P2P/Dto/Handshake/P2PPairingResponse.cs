namespace Beacon.Sdk.Core.Domain.P2P.Dto.Handshake
{
    public record P2PPairingResponse(
        string Id,
        string Name,
        string PublicKey,
        string RelayServer,
        string? Icon = null,
        string? AppUrl = null,
        string Type = "p2p-pairing-response",
        string Version = BeaconConstants.BeaconVersion);
}