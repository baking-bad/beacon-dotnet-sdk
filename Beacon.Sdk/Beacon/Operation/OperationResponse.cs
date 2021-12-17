namespace Beacon.Sdk.Beacon.Operation
{
    using Core.Domain;

    public class OperationResponse : BaseBeaconMessage, IBeaconResponse
    {
        public OperationResponse(string id, string transactionHash)
            : base(BeaconMessageType.operation_response, id)
        {
            TransactionHash = transactionHash;
        }

        public string TransactionHash { get; }
    }
}