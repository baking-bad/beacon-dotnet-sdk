namespace Beacon.Sdk.Core.Domain.Interfaces
{
    using System.Threading.Tasks;
    using Beacon;
    using Entities;
    using Entities.P2P;
    using P2P;

    public delegate Task TaskEventHandler<TEventArgs>(object? sender, TEventArgs e);

    public interface IP2PCommunicationService
    {
        bool LoggedIn { get; }

        bool Syncing { get; }
        event TaskEventHandler<P2PMessageEventArgs> OnP2PMessagesReceived;

        Task LoginAsync(string[] knownRelayServers);

        void Start();

        void Stop();

        Task<P2PPeerRoom> SendChannelOpeningMessageAsync(Peer peer, string id, string appName, string? appUrl,
            string? appIcon);

        Task<P2PPairingRequest> GetPairingRequestInfo(string appName, string[] knownRelayServers, string? iconUrl,
            string? appUrl);

        Task SendMessageAsync(Peer peer, string message);

        Task DeleteAsync(Peer peer);
    }
}