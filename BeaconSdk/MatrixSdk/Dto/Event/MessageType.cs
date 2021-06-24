namespace MatrixSdk.Dto.Event
{
    using Newtonsoft.Json;

    public enum MessageType
    {
        [JsonProperty("m.text")] Text
    }
}