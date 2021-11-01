namespace Beacon.Sdk.Core.Transport.P2P.Dto.Handshake
{
    using Matrix.Sdk.Core.Utils;

    public record PairingResponse(
        string Id,
        HexString ReceiverPublicKeyHex,
        string ReceiverRelayServer,
        int Version);
}