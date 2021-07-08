namespace MatrixSdk.Dto.Sync.Event.Room
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public record BaseRoomEvent(
            RoomEventType Type,
            JObject VariadicContent,
            string SenderUserId,
            string Id,
            long OriginHomeServerTimestamp,
            string RoomId)
        : BaseEvent(Type, VariadicContent)
    {
        /// <summary>
        ///     <b>Required.</b> Contains the fully-qualified ID of the user who sent this event.
        /// </summary>
        [JsonProperty("sender")] public string SenderUserId { get; } = SenderUserId;

        /// <summary>
        ///     <b>Required.</b> The globally unique event identifier.
        /// </summary>
        [JsonProperty("event_id")] public string Id { get; } = Id;

        /// <summary>
        ///     <b>Required.</b> Timestamp in milliseconds on originating homeserver when this event was sent.
        /// </summary>
        [JsonProperty("origin_server_ts")] public long OriginHomeServerTimestamp { get; } = OriginHomeServerTimestamp;

        /// <summary>
        ///     <b>Required.</b> The ID of the room associated with this event.
        ///     Will not be present on events that arrive through /sync, despite being required everywhere else.
        /// </summary>
        public string RoomId { get; } = RoomId;
    }
}