namespace MatrixSdk.Dto.Room.Sync.Event.State.Content
{
    using Newtonsoft.Json;

    /// <remarks>
    ///     m.room.create
    /// </remarks>
    public record RoomCreateContent(string CreatorUserId, bool Federate, string? RoomVersion, PreviousRoom Predecessor)
    {
        /// <summary>
        ///     <b>Required.</b> The user_id of the room creator.
        ///     This is set by the homeserver.
        /// </summary>
        [JsonProperty("creator")] public string CreatorUserId { get; } = CreatorUserId;

        /// <summary>
        ///     Whether users on other servers can join this room. Defaults to <b>true</b> if key does not exist.
        /// </summary>
        [JsonProperty("m.federate")] public bool Federate { get; } = Federate;

        /// <summary>
        ///     The version of the room. Defaults to <b>"1"</b> if the key does not exist.
        /// </summary>
        public string? RoomVersion { get; } = RoomVersion;


        /// <summary>
        ///     A reference to the room this room replaces, if the previous room was upgraded.
        /// </summary>
        public PreviousRoom Predecessor { get; } = Predecessor;
    }

}