namespace Beacon.Sdk.Beacon.Operation
{
    using Permission;

    public record OperationRequest(
            BeaconMessageType Type, 
            string Version, 
            string Id, 
            string SenderId,
            Network Network, 
            string SourceAddress) 
        : BeaconBaseMessage(
            Type,
            Version,
            Id,
            SenderId)
    {
        /// <summary>
        /// Network on which the operation will be broadcast
        /// </summary>
        public Network Network { get; } = Network;

        public string SourceAddress { get; } = SourceAddress;
    }
}