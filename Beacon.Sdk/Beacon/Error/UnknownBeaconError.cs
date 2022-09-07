namespace Beacon.Sdk.Beacon.Error
{
    public record UnknownBeaconError : BaseBeaconError
    {
        public UnknownBeaconError(string id, string senderId) : base(id, senderId,
            BeaconErrorType.UNKNOWN_ERROR)
        {
        }
    }
}