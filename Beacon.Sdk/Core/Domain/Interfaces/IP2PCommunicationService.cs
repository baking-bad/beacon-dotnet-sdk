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

        Task<BeaconPeerRoom> SendChannelOpeningMessageAsync(string id, BeaconPeer beaconPeer, string appName);
        
        Task SendMessageAsync(string roomId, string message);
    }
}