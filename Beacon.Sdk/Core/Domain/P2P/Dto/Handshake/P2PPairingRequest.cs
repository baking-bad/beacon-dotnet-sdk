namespace Beacon.Sdk.Core.Domain.P2P.Dto.Handshake
{
    public record P2PPairingRequest(
        string Id,
        string Name,
        string PublicKey,
        string RelayServer,
        string? Icon = null,
        string? AppUrl = null,
        string Type = "p2p-pairing-request",
        string Version = Constants.BeaconVersion);
}