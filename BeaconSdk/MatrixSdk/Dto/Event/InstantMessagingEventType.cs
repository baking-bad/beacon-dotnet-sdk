namespace MatrixSdk.Dto.Event
{
    using System.Runtime.Serialization;

    public enum InstantMessagingEventType
    {
        [EnumMember(Value = "m.room.message")] Message
    }
}