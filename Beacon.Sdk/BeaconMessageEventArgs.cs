namespace Beacon.Sdk
{
    using System;
    using Core.Beacon;

    public class BeaconMessageEventArgs : EventArgs
    {
        public BeaconMessageEventArgs(BeaconBaseMessage beaconBaseMessage)
        {
            BeaconBaseMessage = beaconBaseMessage;
        }

        public BeaconBaseMessage BeaconBaseMessage { get; }
    }
}