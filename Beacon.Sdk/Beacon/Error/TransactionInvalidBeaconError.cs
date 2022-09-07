namespace Beacon.Sdk.Beacon.Error
{
    public record TransactionInvalidBeaconError : BaseBeaconError
    {
        public TransactionInvalidBeaconError(string id, string senderId) : base(id, senderId,
            BeaconErrorType.TRANSACTION_INVALID_ERROR)
        {
        }
    }
}