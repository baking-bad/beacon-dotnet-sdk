namespace Beacon.Sdk.Core
{
    using System;
    using System.Threading.Tasks;
    using Matrix.Sdk.Core.Utils;
    using Sodium;

    public interface IP2PClient
    {
        Task StartAsync(KeyPair keyPair);

        void ListenToHexPublicKey(HexString hexPublicKey, Action<string> messageCallback);

        void RemoveListenerForPublicKey(HexString hexPublicKey);

        Task SendChannelOpeningMessageAsync(string id, HexString receiverHexPublicKey, string receiverRelayServer, int version);
    }
}