namespace Beacon.Sdk
{
    using System;
    using System.Threading.Tasks;
    using Beacon;
    using Core.Domain;

    public interface IWalletBeaconClient
    {
        bool LoggedIn { get; }
        
        bool Connected { get; }
        
        AppMetadata Metadata { get; }

        IAppMetadataRepository AppMetadataRepository { get; }

        event EventHandler<BeaconMessageEventArgs> OnBeaconMessageReceived;

        Task SendResponseAsync(string receiverId, IBeaconResponse response);

        Task InitAsync();

        Task AddPeerAsync(P2PPairingRequest pairingRequest, bool sendPairingResponse = true);

        void Connect();

        void Disconnect();
    }
}