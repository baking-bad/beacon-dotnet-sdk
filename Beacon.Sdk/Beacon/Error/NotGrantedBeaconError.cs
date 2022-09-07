namespace Beacon.Sdk.Beacon.Error
{
    public record NotGrantedBeaconError : BaseBeaconError
    {
        public NotGrantedBeaconError(string id, string senderId) : base(id, senderId,
            BeaconErrorType.NOT_GRANTED_ERROR)
        {
        }
    }
}