namespace MatrixSdk.Dto.RoomEvent.RoomStateEvent.Content
{
    using Newtonsoft.Json;

    ///<remarks>
    ///m.room.member
    ///</remarks> 
    public record RoomMemberContent(string? AvatarUrl, string? DisplayName, RoomMembership Membership, bool Direct)

    {
        /// <summary>
        /// The avatar URL for this user, if any.
        /// </summary>
        public string? AvatarUrl { get; init; } = AvatarUrl;
        
        /// <summary>
        /// The display name for this user, if any.
        /// </summary>
        [JsonProperty("Displayname")] public string? DisplayName { get; init; } = DisplayName;
        
        /// <summary>
        /// <b>Required.</b> The membership state of the user. One of: ["invite", "join", "knock", "leave", "ban"]
        /// </summary>
        public RoomMembership Membership { get; init; } = Membership;
       
        /// <summary>
        /// Flag indicating if the room containing this event was created with the intention of being a direct chat.
        /// </summary> 
        [JsonProperty("is_direct")] public bool Direct { get; init; } = Direct;
    }
}