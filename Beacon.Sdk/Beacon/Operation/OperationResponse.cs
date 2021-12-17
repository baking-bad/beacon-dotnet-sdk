namespace Beacon.Sdk.Beacon.Operation
{
    using Core.Domain;

    public record OperationResponse : BaseBeaconMessage, IBeaconResponse
    {
        public OperationResponse(string id, string senderId, string transactionHash)
            : base(BeaconMessageType.operation_response, id, senderId)
        {
            TransactionHash = transactionHash;
        }

        public string TransactionHash { get; }
    }
}