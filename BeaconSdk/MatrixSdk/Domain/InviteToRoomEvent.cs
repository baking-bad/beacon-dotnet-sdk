namespace MatrixSdk.Domain
{
    using Dto.Sync.Event.Room;
    using Dto.Sync.Event.Room.State;

    public record InviteToRoomEvent : BaseRoomEvent
    {
        private InviteToRoomEvent(string eventId, string roomId, long originServerTimestamp, string senderUserId)
            : base(eventId, roomId, originServerTimestamp, senderUserId)
        {
        }

        public static class Factory
        {
            public static bool TryBuildFrom(RoomEvent roomEvent, string roomId, out InviteToRoomEvent? inviteToRoomEvent)
            {
                var content = roomEvent.Content.ToObject<RoomMemberContent>();
                if (content == null || content.UserMembershipState != UserMembershipState.Invite)
                {
                    inviteToRoomEvent = null;
                    return false;
                }

                inviteToRoomEvent = new InviteToRoomEvent(
                    roomEvent.EventId, roomId,
                    roomEvent.OriginServerTimestamp,
                    roomEvent.SenderUserId);

                return true;
            }
        }
    }
}