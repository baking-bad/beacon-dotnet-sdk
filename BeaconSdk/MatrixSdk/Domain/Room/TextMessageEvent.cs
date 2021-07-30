namespace MatrixSdk.Domain.Room
{
    using Infrastructure.Dto.Sync.Event.Room;
    using Infrastructure.Dto.Sync.Event.Room.Messaging;

    public record TextMessageEvent(string RoomId, string SenderUserId, string Message) : BaseRoomEvent(RoomId, SenderUserId)
    {
        public static class Factory
        {
            public static bool TryCreateFrom(RoomEvent roomEvent, string roomId, out TextMessageEvent textMessageEvent)
            {
                var content = roomEvent.Content.ToObject<MessageContent>();
                if (content == null || content.MessageType != MessageType.Text)
                {
                    textMessageEvent = new TextMessageEvent(string.Empty, string.Empty, string.Empty);
                    return false;
                }

                textMessageEvent = new TextMessageEvent(roomId, roomEvent.SenderUserId, content.Body);
                return true;
            }

            public static bool TryCreateFromStrippedState(RoomStrippedState roomStrippedState, string roomId, out TextMessageEvent textMessageEvent)
            {
                var content = roomStrippedState.Content.ToObject<MessageContent>();
                if (content == null || content.MessageType != MessageType.Text)
                {
                    textMessageEvent = new TextMessageEvent(string.Empty, string.Empty, string.Empty);
                    return false;
                }

                textMessageEvent = new TextMessageEvent(roomId, roomStrippedState.SenderUserId, content.Body);
                return true;
            }
        }
    }
}