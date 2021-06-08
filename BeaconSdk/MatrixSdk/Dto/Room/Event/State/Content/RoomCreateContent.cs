namespace MatrixSdk.Dto.Room.Event.State.Content
{
    using Newtonsoft.Json;

    ///<remarks>
    ///m.room.create
    ///</remarks>
    public record RoomCreateContent(string Creator, bool Federate, string? RoomVersion, PreviousRoom Predecessor)
    {
        /// <summary>
        /// <b>Required.</b> The user_id of the room creator.
        /// This is set by the homeserver.
        /// </summary>
        public string Creator { get; } = Creator;

        /// <summary>
        /// Whether users on other servers can join this room. Defaults to <b>true</b> if key does not exist.
        /// </summary>
        [JsonProperty("m.federate")] public bool Federate { get; } = Federate;

        /// <summary>
        /// The version of the room. Defaults to <b>"1"</b> if the key does not exist.
        /// </summary>
        public string? RoomVersion { get; } = RoomVersion;


        /// <summary>
        /// A reference to the room this room replaces, if the previous room was upgraded.
        /// </summary>
        public PreviousRoom Predecessor { get; } = Predecessor;
    }

}