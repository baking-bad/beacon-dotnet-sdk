namespace MatrixSdk.Dto.Event
{
    using Newtonsoft.Json;

    public record MessageEvent(MessageType messageType, string Message)
    {
        [JsonProperty("msgtype")] public MessageType messageType { get; } = messageType;

        [JsonProperty("body")] public string Message { get; } = Message;
    }
}