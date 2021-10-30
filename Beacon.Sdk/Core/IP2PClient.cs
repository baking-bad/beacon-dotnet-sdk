namespace Beacon.Sdk.Core
{
    using System.Threading.Tasks;
    using Matrix.Sdk.Core.Utils;
    using Sodium;
    using Transport.P2P.Dto.Handshake;

    public interface IP2PClient
    {
        Task StartAsync(KeyPair keyPair);

        void ListenToPublicKeyHex(HexString publicKey, EncryptedMessageListener listener);

        void RemoveListenerForPublicKey(HexString publicKey);

        Task SendPairingResponseAsync(PairingResponse response);
    }
}