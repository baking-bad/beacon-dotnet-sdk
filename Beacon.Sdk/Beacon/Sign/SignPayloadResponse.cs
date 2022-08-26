namespace Beacon.Sdk.Beacon.Sign
{
    public record SignPayloadResponse : BaseBeaconMessage
    {
        public SignPayloadResponse(
            string signature,
            string version,
            string id,
            string senderId
        ) : base(BeaconMessageType.sign_payload_response, version, id, senderId)
        {
            Signature = signature;
        }

        public string Signature { get; }
    }
}