namespace Beacon.Sdk.Beacon.Operation
{
    public record OperationResponse : BaseBeaconMessage
    {
        public OperationResponse(string id, string senderId, string transactionHash)
            : base(BeaconMessageType.operation_response, id, senderId)
        {
            TransactionHash = transactionHash;
        }

        public string TransactionHash { get; }
    }
}