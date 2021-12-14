namespace Beacon.Sdk.Beacon.Operation
{
    using Core.Domain;

    public record OperationResponse(
            string Id,
            string SenderId,
            string TransactionHash)
        : BeaconBaseMessage(
            BeaconMessageType.operation_response,
            Constants.MessageVersion,
            Id,
            SenderId), IBeaconResponse
    {
        public OperationResponse(string id, string transactionHash)
            : this(id, string.Empty, transactionHash)
        {
        }

        public string TransactionHash { get; } = TransactionHash;
    }
}