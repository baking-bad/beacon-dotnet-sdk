namespace Beacon.Sdk
{
    using System;
    using Beacon;

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
}