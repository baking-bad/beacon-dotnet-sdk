namespace Beacon.Sdk.Core.Domain.Interfaces
{
    using System;
    using System.Threading.Tasks;
    using Matrix.Sdk.Core.Utils;
    using Sodium;
    using Transport.P2P;

    public interface IP2PCommunicationClient
    {
        event EventHandler<P2PMessageEventArgs> OnP2PMessagesReceived;
        
        Task LoginAsync(KeyPair keyPair);

        void Start();

        void Stop();

        Task SendChannelOpeningMessageAsync(string id, HexString receiverHexPublicKey,
            string receiverRelayServer, int version, string appName);
    }
}