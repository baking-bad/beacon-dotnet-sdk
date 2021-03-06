namespace Beacon.Sdk.Beacon
{
    public record BaseBeaconMessage
    {
        // public for json deserialize
        public BaseBeaconMessage(BeaconMessageType type, string version, string id, string senderId)
        {
            Type = type;
            Version = version;
            Id = id;
            SenderId = senderId;
        }

        protected BaseBeaconMessage(BeaconMessageType type, string id, string senderId)
        {
            Type = type;
            Version = Constants.MessageVersion;
            Id = id;
            SenderId = senderId;
        }

        /// <summary>
        ///     Type of beacon message by tzip-10
        ///     <a href="https://gitlab.com/tezos/tzip/-/blob/master/proposals/tzip-10/tzip-10.md">tzip-10 link</a>
        /// </summary>
        public BeaconMessageType Type { get; }

        /// <summary>
        ///     Version of Beacon message.
        /// </summary>
        public string Version { get; }

        /// <summary>
        ///     Id of the message. The same ID is used in the request and response
        /// </summary>
        public string Id { get; }

        /// <summary>
        ///     Id of the sender. This is used to identify the sender of the message
        /// </summary>
        public string SenderId { get; }
    }
}