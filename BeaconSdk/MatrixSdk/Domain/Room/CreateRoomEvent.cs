namespace MatrixSdk.Domain.Room
{
    using Infrastructure.Dto.Sync.Event.Room;
    using Infrastructure.Dto.Sync.Event.Room.State;

    public record CreateRoomEvent(string RoomId, string SenderUserId, string RoomCreatorUserId) : BaseRoomEvent(RoomId, SenderUserId)
    {
        public static class Factory
        {
            public static bool TryCreateFrom(RoomEvent roomEvent, string roomId, out CreateRoomEvent createRoomEvent)
            {
                var content = roomEvent.Content.ToObject<RoomCreateContent>();
                if (content == null)
                {
                    createRoomEvent = new CreateRoomEvent(string.Empty, string.Empty, string.Empty);
                    return false;
                }

                createRoomEvent = new CreateRoomEvent(roomId, roomEvent.SenderUserId, content.RoomCreatorUserId);
                return true;
            }

            public static bool TryCreateFromStrippedState(RoomStrippedState roomStrippedState, string roomId, out CreateRoomEvent createRoomEvent)
            {
                var content = roomStrippedState.Content.ToObject<RoomCreateContent>();
                if (content == null)
                {
                    createRoomEvent = new CreateRoomEvent(string.Empty, string.Empty, string.Empty);
                    return false;
                }

                createRoomEvent = new CreateRoomEvent(roomId, roomStrippedState.SenderUserId, content.RoomCreatorUserId);
                return true;
            }
        }
    }
}