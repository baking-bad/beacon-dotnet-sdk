namespace Beacon.Sdk
{
    using System;
    using Beacon;

    public class BeaconMessageEventArgs : EventArgs
    {
        public BeaconMessageEventArgs(string senderId, BeaconBaseMessage beaconBaseMessage)
        {
            SenderId = senderId;
            BeaconBaseMessage = beaconBaseMessage;
        }

        public string SenderId { get; }

        public BeaconBaseMessage BeaconBaseMessage { get; }
    }
}