namespace BeaconSdk.Domain.Beacon.BaseMessage
{
    public abstract record BeaconBaseMessage(
        BeaconMessageType Type,
        string Version,
        string Id, // ID of the message. The same ID is used in the request and response
        string SenderId // ID of the sender. This is used to identify the sender of the message. This should be the hash of the public key of the peer.
    );
}