namespace MatrixSdk.Domain
{
    using Dto.Sync.Event.Room;
    using Dto.Sync.Event.Room.State;

    public record InviteToRoomEvent : BaseRoomEvent
    {
        private InviteToRoomEvent(string roomId, string senderUserId)
            : base(roomId, senderUserId)
        {
        }

        public static class Factory
        {
            public static bool TryCreateFrom(RoomEvent roomEvent, string roomId, out InviteToRoomEvent? inviteToRoomEvent)
            {
                var content = roomEvent.Content.ToObject<RoomMemberContent>();
                if (content == null || content.UserMembershipState != UserMembershipState.Invite)
                {
                    inviteToRoomEvent = null;
                    return false;
                }

                inviteToRoomEvent = new InviteToRoomEvent(roomId, roomEvent.SenderUserId);
                return true;
            }
        }
    }
}