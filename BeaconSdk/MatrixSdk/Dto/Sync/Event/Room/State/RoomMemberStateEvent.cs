namespace MatrixSdk.Dto.Sync.Event.Room.State
{
    using System;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public record RoomMemberStateEvent (
        RoomEventType Type,
        JObject VariadicContent,
        string SenderUserId,
        string Id,
        long OriginHomeServerTimestamp,
        string RoomId,
        string StateKey) : RoomStateEvent(Type, VariadicContent, SenderUserId, Id, OriginHomeServerTimestamp, RoomId, StateKey)
    {

        public enum UserMembershipState
        {
            Invite,
            Join,
            Knock,
            Leave,
            Ban
        }

        public Content GetContent()
        {
            if (Type != RoomEventType.Member)
                throw new InvalidOperationException($"Invalid operation: Content is not {nameof(Content)}");

            return VariadicContent.ToObject<Content>() ??
                   throw new InvalidOperationException($"Cannot parse {nameof(Content)}");
        }

        /// <remarks>
        ///     m.room.member
        /// </remarks>
        public record Content(string? AvatarUrl, string? UserDisplayName, UserMembershipState UserMembershipState, bool IsDirectChat)

        {
            /// <summary>
            ///     The avatar URL for this user, if any.
            /// </summary>
            public string? AvatarUrl { get; } = AvatarUrl;

            /// <summary>
            ///     The display name for this user, if any.
            /// </summary>
            [JsonProperty("displayname")] public string? UserDisplayName { get; } = UserDisplayName;

            /// <summary>
            ///     <b>Required.</b> The membership state of the user. One of: ["invite", "join", "knock", "leave", "ban"]
            /// </summary>
            [JsonProperty("membership")] public UserMembershipState UserMembershipState { get; } = UserMembershipState;

            /// <summary>
            ///     Flag indicating if the room containing this event was created with the intention of being a direct chat.
            /// </summary>
            [JsonProperty("is_direct")] public bool IsDirectChat { get; } = IsDirectChat;
        }
    }
}