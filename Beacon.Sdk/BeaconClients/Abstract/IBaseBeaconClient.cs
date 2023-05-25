namespace Beacon.Sdk.BeaconClients.Abstract
{
    using System;
    using System.Threading.Tasks;
    using Beacon;
    using Core.Domain.Interfaces.Data;

    public interface IBaseBeaconClient
    {
        event EventHandler<BeaconMessageEventArgs> OnBeaconMessageReceived;
        event EventHandler<ConnectedClientsListChangedEventArgs?> OnConnectedClientsListChanged;
        event Action OnDisconnected;

        Task InitAsync();
        void Connect();
        void Disconnect();
        Task SendResponseAsync(string receiverId, BaseBeaconMessage response);
        Task RemovePeerAsync(string peerSenderId);

        string AppName { get; }
        string SenderId { get; }
        bool LoggedIn { get; }
        bool Connected { get; }
        AppMetadata Metadata { get; }
        IAppMetadataRepository AppMetadataRepository { get; }
        IPermissionInfoRepository PermissionInfoRepository { get; }
    }
}