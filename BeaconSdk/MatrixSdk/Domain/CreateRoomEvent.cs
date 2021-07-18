namespace MatrixSdk.Domain
{
    using Dto.Sync.Event.Room;
    using Dto.Sync.Event.Room.State;

    public record CreateRoomEvent : BaseRoomEvent
    {
        public CreateRoomEvent(string eventId, string roomId, long originServerTimestamp, string senderUserId, string roomCreatorUserId) :
            base(eventId, roomId, originServerTimestamp, senderUserId)
        {
            RoomCreatorUserId = roomCreatorUserId;
        }

        public string RoomCreatorUserId { get; }

        public static class Factory
        {
            public static bool TryBuildFrom(RoomEvent roomEvent, string roomId, out CreateRoomEvent? createRoomEvent)
            {
                var content = roomEvent.Content.ToObject<RoomCreateContent>();
                if (content == null)
                {
                    createRoomEvent = null;
                    return false;
                }

                createRoomEvent = new CreateRoomEvent(
                    roomEvent.EventId,
                    roomId,
                    roomEvent.OriginServerTimestamp,
                    roomEvent.SenderUserId,
                    content.RoomCreatorUserId);

                return true;
            }
        }
    }
}