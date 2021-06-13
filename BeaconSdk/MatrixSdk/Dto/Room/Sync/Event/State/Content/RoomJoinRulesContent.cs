namespace MatrixSdk.Dto.Room.Sync.Event.State.Content
{
    using Newtonsoft.Json;

    /// <remarks>
    ///     m.room.join_rules
    /// </remarks>
    public record RoomJoinRulesContent(JoinRule JoinRule)
    {
        /// <summary>
        ///     <b>Required.</b> The type of rules used for users wishing to join this room.
        ///     One of: ["public", "knock", "invite", "private"]
        /// </summary>
        [JsonProperty("join_rule")] public JoinRule JoinRule { get; } = JoinRule;
    }
}