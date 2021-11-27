namespace Beacon.Sdk
{
    using System;
    using System.Threading.Tasks;
    using Beacon;

    public interface IWalletClient
    {
        AppMetadata Metadata { get; }

        IAppMetadataRepository AppMetadataRepository { get; }

        event EventHandler<BeaconMessageEventArgs> OnBeaconMessageReceived;

        Task SendMessageAsync(string receiverId, BeaconBaseMessage baseMessage);

        Task InitAsync();

        Task AddPeerAsync(P2PPairingRequest pairingRequest, bool sendPairingResponse = true);

        void Connect();

        void Disconnect();
    }
}