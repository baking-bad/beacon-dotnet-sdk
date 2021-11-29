namespace Beacon.Sdk
{
    using System;
    using Core.Domain;

    public class BeaconMessageEventArgs : EventArgs
    {
        public BeaconMessageEventArgs(string senderId, IBeaconRequest request)
        {
            SenderId = senderId;
            Request = request;
        }

        public string SenderId { get; }

        public IBeaconRequest Request { get; }
    }
}