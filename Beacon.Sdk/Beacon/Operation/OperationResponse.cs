namespace Beacon.Sdk.Beacon.Operation
{
    public record OperationResponse : BaseBeaconMessage
    {
        public OperationResponse(string id, string senderId, string transactionHash, string version)
            : base(BeaconMessageType.operation_response, version, id, senderId)
        {
            TransactionHash = transactionHash;
        }

        public string TransactionHash { get; }
    }
}