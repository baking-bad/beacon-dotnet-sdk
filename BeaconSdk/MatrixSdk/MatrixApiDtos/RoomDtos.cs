namespace MatrixSdk.MatrixApiDtos
{
    using System.Runtime.Serialization;

    public enum Visibility
    {
        [EnumMember(Value = "public")] Public,

        [EnumMember(Value = "private")] Private
    }

    public enum Preset
    {
        [EnumMember(Value = "private_chat")] PrivateChat,

        [EnumMember(Value = "public_chat")] PublicChat,

        [EnumMember(Value = "trusted_private_chat")]
        TrustedPrivateChat
    }

    public record RequestCreateRoomDto(
        Visibility? Visibility = null,
        string? RoomAliasName = null,
        string? Name = null,
        string? Topic = null,
        string[]? Invite = null,
        string? RoomVersion = null, 
        Preset? Preset = null,
        bool? IsDirect = null);

    public record ResponseCreateRoomDto(string? RoomId = null);
}