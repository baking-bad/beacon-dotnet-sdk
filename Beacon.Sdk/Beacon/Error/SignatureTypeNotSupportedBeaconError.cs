namespace Beacon.Sdk.Beacon.Error
{
    public record SignatureTypeNotSupportedBeaconError : BaseBeaconError
    {
        public SignatureTypeNotSupportedBeaconError(string id, string senderId) : base(id, senderId,
            BeaconErrorType.SIGNATURE_TYPE_NOT_SUPPORTED)
        {
        }
    }
}