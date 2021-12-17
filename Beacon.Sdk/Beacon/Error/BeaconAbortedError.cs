namespace Beacon.Sdk.Beacon.Error
{
    public record BeaconAbortedError : BaseBeaconError
    {
        public BeaconAbortedError(string id, string senderId) : base(id, senderId, BeaconErrorType.ABORTED_ERROR)
        {
        }
    }
}