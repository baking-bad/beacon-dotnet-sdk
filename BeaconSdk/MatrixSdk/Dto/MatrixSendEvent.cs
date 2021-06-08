namespace MatrixSdk.Dto
{
    // Todo: rename msgtype
    public record MessageEvent(string msgtype, string messageType = "m.text")
    {
        public string msgtype { get; } = msgtype;
        public string messageType { get; } = messageType;
    }

    public record EventResponse(string? EventId)
    {
        public string? EventId { get; } = EventId;
    }
}