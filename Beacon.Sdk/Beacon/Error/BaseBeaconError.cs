namespace Beacon.Sdk.Beacon.Error
{
    using Core.Domain;

    public class BaseBeaconError : BaseBeaconMessage, IBeaconResponse
    {
        protected BaseBeaconError(string id, BeaconErrorType errorType)
            : base(BeaconMessageType.error, id)
        {
            ErrorType = errorType;
        }

        public BeaconErrorType ErrorType { get; }
    }
}