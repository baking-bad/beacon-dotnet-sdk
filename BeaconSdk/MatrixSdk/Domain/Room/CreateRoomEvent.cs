namespace MatrixSdk.Domain.Room
{
    using Infrastructure.Dto.Sync.Event.Room;
    using Infrastructure.Dto.Sync.Event.Room.State;

    public record CreateRoomEvent : BaseRoomEvent
    {
        private CreateRoomEvent(string roomId, string senderUserId, string roomCreatorUserId) :
            base(roomId, senderUserId)
        {
            RoomCreatorUserId = roomCreatorUserId;
        }

        public string RoomCreatorUserId { get; }

        public static class Factory
        {
            public static bool TryCreateFrom(RoomEvent roomEvent, string roomId, out CreateRoomEvent? createRoomEvent)
            {
                var content = roomEvent.Content.ToObject<RoomCreateContent>();
                if (content == null)
                {
                    createRoomEvent = null;
                    return false;
                }

                createRoomEvent = new CreateRoomEvent(roomId, roomEvent.SenderUserId, content.RoomCreatorUserId);
                return true;
            }
        }
    }
}