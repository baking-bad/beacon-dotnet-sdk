namespace Beacon.Sdk.Beacon.Error
{
    public record NetworkNotSupportedBeaconError : BaseBeaconError
    {
        public NetworkNotSupportedBeaconError(string id, string senderId) : base(id, senderId,
            BeaconErrorType.NETWORK_NOT_SUPPORTED_ERROR)
        {
        }
    }
}