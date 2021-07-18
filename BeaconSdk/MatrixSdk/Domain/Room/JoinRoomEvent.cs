namespace MatrixSdk.Domain.Room
{
    using Infrastructure.Dto.Sync.Event.Room;
    using Infrastructure.Dto.Sync.Event.Room.State;

    public record JoinRoomEvent : BaseRoomEvent
    {
        private JoinRoomEvent(string roomId, string senderUserId)
            : base(roomId, senderUserId)
        {
        }

        public static class Factory
        {
            public static bool TryCreateFrom(RoomEvent roomEvent, string roomId, out JoinRoomEvent? joinRoomEvent)
            {
                var content = roomEvent.Content.ToObject<RoomMemberContent>();
                if (content == null || content.UserMembershipState != UserMembershipState.Join)
                {
                    joinRoomEvent = null;
                    return false;
                }

                joinRoomEvent = new JoinRoomEvent(roomId,
                    roomEvent.SenderUserId);

                return true;
            }

            public static bool TryCreateFrom(RoomStrippedState roomStrippedState, string roomId, out JoinRoomEvent? joinRoomEvent)
            {
                var content = roomStrippedState.Content.ToObject<RoomMemberContent>();
                if (content == null || content.UserMembershipState != UserMembershipState.Join)
                {
                    joinRoomEvent = null;
                    return false;
                }

                joinRoomEvent = new JoinRoomEvent(roomId,
                    roomStrippedState.SenderUserId);

                return true;
            }
        }
    }
}