namespace MatrixSdk.Domain.Room
{
    using Infrastructure.Dto.Sync.Event.Room;
    using Infrastructure.Dto.Sync.Event.Room.State;

    public record InviteToRoomEvent(string RoomId, string SenderUserId) : BaseRoomEvent(RoomId, SenderUserId)
    {
        public static class Factory
        {
            public static bool TryCreateFrom(RoomEvent roomEvent, string roomId, out InviteToRoomEvent inviteToRoomEvent)
            {
                var content = roomEvent.Content.ToObject<RoomMemberContent>();
                if (content == null || content.UserMembershipState != UserMembershipState.Invite)
                {
                    inviteToRoomEvent = new InviteToRoomEvent(string.Empty, string.Empty);
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
                    inviteToRoomEvent = new InviteToRoomEvent(string.Empty, string.Empty);
                    return false;
                }

                inviteToRoomEvent = new InviteToRoomEvent(roomId, roomStrippedState.SenderUserId);
                return true;
            }
        }
    }
}