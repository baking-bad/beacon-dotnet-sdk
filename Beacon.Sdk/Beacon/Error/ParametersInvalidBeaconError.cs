namespace Beacon.Sdk.Beacon.Error
{
    public record ParametersInvalidBeaconError : BaseBeaconError
    {
        public ParametersInvalidBeaconError(string id, string senderId) : base(id, senderId,
            BeaconErrorType.PARAMETERS_INVALID_ERROR)
        {
        }
    }
}