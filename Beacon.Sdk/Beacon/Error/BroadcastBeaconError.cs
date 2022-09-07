namespace Beacon.Sdk.Beacon.Error
{
    public record BroadcastBeaconError : BaseBeaconError
    {
        public BroadcastBeaconError(string id, string senderId) : base(id, senderId,
            BeaconErrorType.BROADCAST_ERROR)
        {
        }
    }
}