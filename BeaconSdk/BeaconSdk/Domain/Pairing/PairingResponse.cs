namespace BeaconSdk.Domain.Pairing
{
    public record PairingResponse(
        string Id,
        string Type,
        string Name,
        string Version,
        string PublicKey,
        string RelayServer);
}