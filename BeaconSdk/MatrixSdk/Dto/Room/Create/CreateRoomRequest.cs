namespace MatrixSdk.Dto.Room.Create
{
    public record CreateRoomRequest(
        Visibility? Visibility = null,
        string? RoomAliasName = null,
        string? Name = null,
        string? Topic = null,
        string[]? Invite = null,
        string? RoomVersion = null,
        Preset? Preset = null,
        bool? IsDirect = null)
    {
        public Visibility? Visibility { get; } = Visibility;
        public string? RoomAliasName { get; } = RoomAliasName;
        public string? Name { get; } = Name;
        public string? Topic { get; } = Topic;
        public string[]? Invite { get; } = Invite;
        public string? RoomVersion { get; } = RoomVersion;
        public Preset? Preset { get; } = Preset;
        public bool? IsDirect { get; } = IsDirect;
    }
}