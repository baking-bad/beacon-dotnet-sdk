namespace Beacon.Sdk
{
    using System;
    using Beacon;
    using Core.Domain.Entities;

    public class BeaconMessageEventArgs : EventArgs
    {
        public BeaconMessageEventArgs(BaseBeaconMessage? request, bool pairingDone = false)
        {
            Request = request;
            PairingDone = pairingDone;
        }

        public BaseBeaconMessage? Request { get; }
        public bool PairingDone { get; }
    }

    public class ConnectedClientsListChangedEventArgs : EventArgs
    {
        public ConnectedClientsListChangedEventArgs(AppMetadata metadata, PermissionInfo permissionInfo)
        {
            Metadata = metadata;
            PermissionInfo = permissionInfo;
        }

        public AppMetadata Metadata { get; }
        public PermissionInfo PermissionInfo { get; }
    }
}