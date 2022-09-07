namespace Beacon.Sdk.Beacon.Error
{
    public record TooManyOperationsBeaconError : BaseBeaconError
    {
        public TooManyOperationsBeaconError(string id, string senderId) : base(id, senderId,
            BeaconErrorType.TOO_MANY_OPERATIONS_ERROR)
        {
        }
    }
}