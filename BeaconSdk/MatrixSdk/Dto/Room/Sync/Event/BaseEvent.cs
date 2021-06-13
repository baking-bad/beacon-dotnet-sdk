namespace MatrixSdk.Dto.Room.Sync.Event
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;


    public record BaseEvent(RoomEventType RoomEventType, JObject Content)
    {
        /// <summary>
        ///     <b>Required.</b> The type of event.
        ///     This SHOULD be namespaced similar to Java package naming conventions e.g. 'com.example.subdomain.event.type'
        /// </summary>
        [JsonProperty("type")] public RoomEventType RoomEventType { get; } = RoomEventType;

        /// <summary>
        ///     <b>Required.</b> The fields in this object will vary depending on the type of event.
        ///     When interacting with the REST API, this is the HTTP body.
        /// </summary>
        public JObject Content { get; } = Content;
    }
}