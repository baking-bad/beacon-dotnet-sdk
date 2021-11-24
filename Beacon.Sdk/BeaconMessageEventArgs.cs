namespace Beacon.Sdk
{
    using System;
    using Beacon;

    public class BeaconMessageEventArgs : EventArgs
    {
        public BeaconMessageEventArgs(BeaconBaseMessage beaconBaseMessage)
        {
            BeaconBaseMessage = beaconBaseMessage;
        }

        public BeaconBaseMessage BeaconBaseMessage { get; }
    }
}