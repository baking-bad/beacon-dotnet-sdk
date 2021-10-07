namespace BeaconSdk.Domain.Pairing
{
    public record PairingResponse(
        string Id,
        string Type,
        string Name,
        string Version,
        string PublicKey,
        string RelayServer);
    
    // public record PairingResponse(
    //     string id,
    //     string type,
    //     string name,
    //     string version,
    //     string publicKey,
    //     string relayServer);
}