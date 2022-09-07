namespace Beacon.Sdk.Beacon.Error
{
    public record NoPrivateKeyBeaconError : BaseBeaconError
    {
        public NoPrivateKeyBeaconError(string id, string senderId) : base(id, senderId,
            BeaconErrorType.NO_PRIVATE_KEY_FOUND_ERROR)
        {
        }
    }
}