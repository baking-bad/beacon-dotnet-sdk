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

        string AppName { get; }
        string SenderId { get; }
        bool LoggedIn { get; }
        bool Connected { get; }
        AppMetadata Metadata { get; }
        IAppMetadataRepository AppMetadataRepository { get; }
        IPermissionInfoRepository PermissionInfoRepository { get; }
        Task SendResponseAsync(string receiverId, BaseBeaconMessage response);
        Task InitAsync();
        Task RemovePeerAsync(string peerSenderId);
        void Connect();
        void Disconnect();
    }
}