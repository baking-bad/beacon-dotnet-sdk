namespace MatrixSdk.Dto.Sync.Event.Room.State
{
    using System;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    [Obsolete]
    public record RoomJoinRulesStateEvent (
        EventType Type,
        JObject VariadicContent,
        string SenderUserId,
        string Id,
        long OriginHomeServerTimestamp,
        string RoomId,
        string StateKey) //: RoomStateEvent(Type, VariadicContent, SenderUserId, Id, OriginHomeServerTimestamp, RoomId, StateKey)
    {

        public enum JoinRule
        {
            Public,
            Invite
        }

        // public Content GetContent()
        // {
        //     if (Type != BaseEventType.JoinRules)
        //         throw new InvalidOperationException($"Invalid operation: Content is not {nameof(Content)}");
        //
        //     return VariadicContent.ToObject<Content>() ??
        //            throw new InvalidOperationException($"Cannot parse {nameof(Content)}");
        // }

        /// <remarks>
        ///     m.room.join_rules
        /// </remarks>
        public record Content(JoinRule JoinRule)
        {
            /// <summary>
            ///     <b>Required.</b> The type of rules used for users wishing to join this room.
            ///     One of: ["public", "knock", "invite", "private"]
            /// </summary>
            [JsonProperty("join_rule")] public JoinRule JoinRule { get; } = JoinRule;
        }
    }
}