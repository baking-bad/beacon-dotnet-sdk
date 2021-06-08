namespace MatrixSdk.Dto.Room.Event
{
    using System.Runtime.Serialization;

    public enum RoomEventType
    {
        [EnumMember(Value = "m.room.create")] Create,
        [EnumMember(Value = "m.room.join_rules")] JoinRules,
        [EnumMember(Value = "m.room.member")] Member
    }
}