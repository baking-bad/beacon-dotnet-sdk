namespace Beacon.Sdk.Beacon
{
   
    public record BeaconBaseMessage(BeaconMessageType Type, string Version, string Id, string SenderId)
    {
        /// <summary>
        /// Type of beacon message by tzip-10
        /// <a href="https://gitlab.com/tezos/tzip/-/blob/master/proposals/tzip-10/tzip-10.md">tzip-10 link</a>
        /// </summary>
        public BeaconMessageType Type { get; } = Type;
        
        public string Version { get; } = Version;

        /// <summary>
        /// Id of the message. The same ID is used in the request and response
        /// </summary>
        public string Id { get; } = Id;

        /// <summary>
        /// Id of the sender. This is used to identify the sender of the message
        /// </summary>
        public string SenderId { get; } = SenderId;
    }
}