namespace Beacon.Sdk.Core.Transport.P2P.Dto.Handshake
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

    // Todo: tzip-10??
    // public record P2PPairingRequest(
    //     string Name,
    //     string? Icon,
    //     string? AppUrl,
    //     string PublicKey,
    //     string RelayServer
    // );
}