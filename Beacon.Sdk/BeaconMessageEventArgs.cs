namespace Beacon.Sdk
{
    using System;
    using Beacon;
    using Core.Domain.Entities;

    public class BeaconMessageEventArgs : EventArgs
    {
        public BeaconMessageEventArgs(string senderId, BaseBeaconMessage request)
        {
            SenderId = senderId;
            Request = request;
        }

        public string SenderId { get; }

        public BaseBeaconMessage Request { get; }
    }

    public class DappConnectedEventArgs : EventArgs
    {
        public DappConnectedEventArgs(AppMetadata dappMetadata, PermissionInfo dappPermissionInfo)
        {
            this.dappMetadata = dappMetadata;
            this.dappPermissionInfo = dappPermissionInfo;
        }

        public AppMetadata dappMetadata { get; }

        public PermissionInfo dappPermissionInfo { get; }
    }
}