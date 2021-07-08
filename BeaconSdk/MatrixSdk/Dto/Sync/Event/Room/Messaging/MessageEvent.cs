namespace MatrixSdk.Dto.Sync.Event.Room.Messaging
{
    using Newtonsoft.Json.Linq;

    public record MessageEvent(
            RoomEventType Type,
            JObject Content,
            string SenderUserId,
            string Id,
            long OriginHomeServerTimestamp,
            string RoomId)
        : BaseRoomEvent(Type, Content, SenderUserId, Id, OriginHomeServerTimestamp, RoomId)
    {
        // public MessageContent Content { get; } = Content;
    }
}