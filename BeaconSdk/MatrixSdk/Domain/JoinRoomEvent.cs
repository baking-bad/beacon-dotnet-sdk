namespace MatrixSdk.Domain
{
    using Dto.Sync.Event.Room;
    using Dto.Sync.Event.Room.State;

    public record JoinRoomEvent : BaseRoomEvent
    {
        private JoinRoomEvent(string eventId, string roomId, long originServerTimestamp, string senderUserId)
            : base(eventId, roomId, originServerTimestamp, senderUserId)
        {
        }

        public static class Factory
        {
            public static bool TryBuildFrom(RoomEvent roomEvent, string roomId, out JoinRoomEvent? joinRoomEvent)
            {
                var content = roomEvent.Content.ToObject<RoomMemberContent>();
                if (content == null || content.UserMembershipState != UserMembershipState.Join)
                {
                    joinRoomEvent = null;
                    return false;
                }

                joinRoomEvent = new JoinRoomEvent(
                    roomEvent.EventId,
                    roomId,
                    roomEvent.OriginServerTimestamp,
                    roomEvent.SenderUserId);

                return true;
            }
        }
    }

}