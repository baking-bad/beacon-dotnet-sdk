namespace MatrixSdk.Dto.Event
{
    // Todo: rename msgtype
    public record MessageEvent(string msgtype, string messageType = "m.text")
    {
        public string msgtype { get; } = msgtype;
        public string messageType { get; } = messageType;
    }

}