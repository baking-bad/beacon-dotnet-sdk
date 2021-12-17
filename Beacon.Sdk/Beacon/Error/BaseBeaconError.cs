namespace Beacon.Sdk.Beacon.Error
{
    using Core.Domain;

    public record BaseBeaconError : BaseBeaconMessage, IBeaconResponse
    {
        protected BaseBeaconError(string id, string senderId, BeaconErrorType errorType)
            : base(BeaconMessageType.error, id, senderId)
        {
            ErrorType = errorType;
        }

        public BeaconErrorType ErrorType { get; }
    }
}