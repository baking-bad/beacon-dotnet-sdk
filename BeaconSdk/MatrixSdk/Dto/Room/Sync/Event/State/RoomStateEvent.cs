namespace MatrixSdk.Dto.Room.Sync.Event.State
{
    using Newtonsoft.Json.Linq;

    public record RoomStateEvent(
            RoomEventType Type,
            JObject Content,
            string SenderUserId,
            string Id,
            long OriginHomeServerTimestamp,
            string RoomId,
            string StateKey)
        : BaseRoomEvent(Type, Content, SenderUserId, Id, OriginHomeServerTimestamp, RoomId)
    {
        /// <summary>
        ///     <b>Required.</b>
        ///     A unique key which defines the overwriting semantics for this piece of room state.
        ///     This value is often a zero-length string.
        ///     The presence of this key makes this event a State Event.
        ///     State keys starting with an @ are reserved for referencing user IDs, such as room members.
        ///     With the exception of a few events, state events set with a given user's ID as the state key MUST only be set by
        ///     that user.
        /// </summary>
        public string StateKey { get; set; } = StateKey;
    }
}