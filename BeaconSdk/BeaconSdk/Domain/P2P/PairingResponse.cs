namespace BeaconSdk.Domain.P2P
{
    public record PairingResponse(string Id, string Type, string Name, string Version, string PublicKey, string RelayServer);
}