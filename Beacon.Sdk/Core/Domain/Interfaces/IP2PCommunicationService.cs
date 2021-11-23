namespace Beacon.Sdk.Core.Domain.Interfaces
{
    using System;
    using System.Threading.Tasks;
    using P2P;
    using Utils;

    public interface IP2PCommunicationService
    {
        event EventHandler<P2PMessageEventArgs> OnP2PMessagesReceived;

        Task LoginAsync();

        void Start();

        void Stop();

        Task SendChannelOpeningMessageAsync(string id, HexString receiverHexPublicKey,
            string receiverRelayServer, string version, string appName);

        Task SendMessageAsync();
    }
}