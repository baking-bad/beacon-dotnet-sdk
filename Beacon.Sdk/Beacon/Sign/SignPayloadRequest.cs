namespace Beacon.Sdk.Beacon.Sign
{
    public record SignPayloadRequest : BaseBeaconMessage
    {
        public SignPayloadRequest(
            string id,
            string version,
            string senderId,
            SignPayloadType? signingType,
            string payload,
            string sourceAddress
        )
            : base(BeaconMessageType.sign_payload_request, version, id, senderId)
        {
            SigningType = signingType;
            Payload = payload;
            SourceAddress = sourceAddress;
        }

        public SignPayloadType? SigningType { get; }
        public string Payload { get; }
        public string SourceAddress { get; }
    }
}