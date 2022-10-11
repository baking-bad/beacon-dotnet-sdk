namespace Beacon.Sdk.BeaconClients.Abstract
{
    using System;
    using System.Threading.Tasks;
    using Beacon;

    public interface IDappBeaconClient
    {
        // string SenderId { get; }
        //
        // bool LoggedIn { get; }
        //
        // bool Connected { get; }
        //
        // AppMetadata Metadata { get; }
        //
        // IAppMetadataRepository AppMetadataRepository { get; }
        //
        // IPermissionInfoRepository PermissionInfoRepository { get; }
        //
        event EventHandler<BeaconMessageEventArgs> OnBeaconMessageReceived;
        event EventHandler<DappConnectedEventArgs?> OnDappsListChanged;
        //
        // Task SendResponseAsync(string receiverId, BaseBeaconMessage response);

        Task InitAsync();
        void Connect();
        Task<string> GetPairingRequestInfo();

        // Task AddPeerAsync(P2PPairingRequest pairingRequest, string addressToConnect, bool sendPairingResponse = true);
        //
        // Peer? GetPeer(string senderId);
        //
        // IEnumerable<Peer> GetAllPeers();
        //
        // Task RemovePeerAsync(string peerSenderId);
        //
        // void Connect();
        //
        // void Disconnect();
        //
        // Task<PermissionInfo?> TryReadPermissionInfo(string sourceAddress, string senderId, Network network);
    }
}