namespace MatrixSdk.Dto.Room.Sync.Event.State.Content
{
    using Newtonsoft.Json;

    public record PreviousRoom(string OldRoomId, string LastKnownEventId)
    {
        /// <summary>
        ///     <b>Required.</b> The ID of the old room.
        /// </summary>
        [JsonProperty("room_id")] public string OldRoomId { get; } = OldRoomId;

        /// <summary>
        ///     <b>Required.</b> The event ID of the last known event in the old room.
        /// </summary>
        [JsonProperty("event_id")] public string LastKnownEventId { get; } = LastKnownEventId;
    }
}