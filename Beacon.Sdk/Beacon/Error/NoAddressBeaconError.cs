namespace Beacon.Sdk.Beacon.Error
{
    public record NoAddressBeaconError : BaseBeaconError
    {
        public NoAddressBeaconError(string id, string senderId) : base(id, senderId,
            BeaconErrorType.NO_ADDRESS_ERROR)
        {
        }
    }
}