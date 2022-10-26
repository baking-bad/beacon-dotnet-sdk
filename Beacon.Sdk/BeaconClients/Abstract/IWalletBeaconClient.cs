namespace Beacon.Sdk.BeaconClients.Abstract
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Beacon;
    using Beacon.Permission;
    using Core.Domain.Entities;
    using Core.Domain.Interfaces.Data;

    public interface IWalletBeaconClient
    {
        string SenderId { get; }

        bool LoggedIn { get; }

        bool Connected { get; }

        AppMetadata Metadata { get; }

        IAppMetadataRepository AppMetadataRepository { get; }

        IPermissionInfoRepository PermissionInfoRepository { get; }

        event EventHandler<BeaconMessageEventArgs> OnBeaconMessageReceived;
        
        event EventHandler<ConnectedClientsListChangedEventArgs?> OnConnectedClientsListChanged;

        Task SendResponseAsync(string receiverId, BaseBeaconMessage response);

        Task InitAsync();

        Task AddPeerAsync(P2PPairingRequest pairingRequest, bool sendPairingResponse = true);

        Peer? GetPeer(string senderId);

        Task RemovePeerAsync(string peerSenderId);

        void Connect();

        void Disconnect();

        Task<PermissionInfo?> TryReadPermissionInfo(string sourceAddress, string senderId, Network network);
    }
}