namespace MatrixSdk.Domain.Room
{
    using Infrastructure.Dto.Sync.Event.Room;
    using Infrastructure.Dto.Sync.Event.Room.State;

    public record InviteToRoomEvent : BaseRoomEvent
    {
        private InviteToRoomEvent(string roomId, string senderUserId)
            : base(roomId, senderUserId)
        {
        }

        public static class Factory
        {
            public static bool TryCreateFrom(RoomEvent roomEvent, string roomId, out InviteToRoomEvent inviteToRoomEvent)
            {
                var content = roomEvent.Content.ToObject<RoomMemberContent>();
                if (content == null || content.UserMembershipState != UserMembershipState.Invite)
                {
                    inviteToRoomEvent = new InviteToRoomEvent("", "");
                    return false;
                }

                inviteToRoomEvent = new InviteToRoomEvent(roomId, roomEvent.SenderUserId);
                return true;
            }
            
            public static bool TryCreateFromStrippedState(RoomStrippedState roomStrippedState, string roomId, out InviteToRoomEvent inviteToRoomEvent)
            {
                var content = roomStrippedState.Content.ToObject<RoomMemberContent>();
                if (content == null || content.UserMembershipState != UserMembershipState.Invite)
                {
                    inviteToRoomEvent = new InviteToRoomEvent("", "");
                    return false;
                }

                inviteToRoomEvent = new InviteToRoomEvent(roomId, roomStrippedState.SenderUserId);
                return true;
            }
        }
    }
}