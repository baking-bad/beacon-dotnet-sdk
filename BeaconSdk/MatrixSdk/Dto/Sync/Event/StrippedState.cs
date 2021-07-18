namespace MatrixSdk.Dto.Sync.Event
{
    using Newtonsoft.Json;

    public record StrippedState : BaseEvent
    {
        /// <summary>
        ///     <b>Required.</b> Contains the fully-qualified ID of the user who sent this event.
        /// </summary>
        [JsonProperty("sender")] public string SenderUserId { get; init; }

        /// <summary>
        ///     <b>Required.</b>
        ///     A unique key which defines the overwriting semantics for this piece of room state.
        ///     This value is often a zero-length string.
        ///     The presence of this key makes this event a State Event.
        /// </summary>
        public string StateKey { get; init; }
    }
}