namespace Beacon.Sdk.Core.Domain.Interfaces
{
    using System.Threading.Tasks;
    using Entities;
    using Entities.P2P;
    using P2P;

    public delegate Task TaskEventHandler<TEventArgs>(object? sender, TEventArgs e);

    public interface IP2PCommunicationService
    {
        event TaskEventHandler<P2PMessageEventArgs> OnP2PMessagesReceived;

        bool LoggedIn { get; }
        
        bool Syncing { get; }
        
        Task LoginAsync();

        void Start();

        void Stop();

        Task<P2PPeerRoom> SendChannelOpeningMessageAsync(Peer peer, string id, string appName);

        Task SendMessageAsync(Peer peer, string message);
    }
}