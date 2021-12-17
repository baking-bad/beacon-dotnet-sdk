namespace Beacon.Sdk.Beacon.Error
{
    public record BaseBeaconError : BaseBeaconMessage
    {
        protected BaseBeaconError(string id, string senderId, BeaconErrorType errorType)
            : base(BeaconMessageType.error, id, senderId)
        {
            ErrorType = errorType;
        }

        public BeaconErrorType ErrorType { get; }
    }
}