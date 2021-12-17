namespace Beacon.Sdk.Beacon.Error
{
    public class BeaconAbortedError : BaseBeaconError
    {
        public BeaconAbortedError(string id) : base(id, BeaconErrorType.ABORTED_ERROR)
        {
        }
    }
}