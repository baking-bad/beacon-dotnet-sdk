namespace BeaconSdk.Domain.Pairing
{
    public record P2PPairingRequest(
        string Id,
        string Type,
        string Name,
        string PublicKey,
        string RelayServer,
        string? Icon,
        string? AppUrl); //Todo: refactor appUrl
}