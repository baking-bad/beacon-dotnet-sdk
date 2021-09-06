namespace MatrixSdk.Domain
{
    using System.Collections.Generic;
    using Infrastructure.Dto.Sync;
    using Room;

    public class MatrixRoomFactory
    {
        public MatrixRoom CreateJoined(string roomId, JoinedRoom joinedRoom)
        {
            var joinedUserIds = new List<string>();
            foreach (var timelineEvent in joinedRoom.Timeline.Events)
                if (JoinRoomEvent.Factory.TryCreateFrom(timelineEvent, roomId, out var joinRoomEvent))
                    joinedUserIds.Add(joinRoomEvent!.SenderUserId);

            return new MatrixRoom(roomId, MatrixRoomStatus.Joined, joinedUserIds);
        }

        public MatrixRoom CreateInvite(string roomId, InvitedRoom invitedRoom)
        {
            var joinedUserIds = new List<string>();
            foreach (var timelineEvent in invitedRoom.InviteState.Events)
                if (JoinRoomEvent.Factory.TryCreateFromStrippedState(timelineEvent, roomId, out var joinRoomEvent))
                    joinedUserIds.Add(joinRoomEvent!.SenderUserId);

            return new MatrixRoom(roomId, MatrixRoomStatus.Joined, joinedUserIds);
        }

        public MatrixRoom CreateLeft(string roomId, LeftRoom leftRoom)
        {
            var joinedUserIds = new List<string>();
            foreach (var timelineEvent in leftRoom.Timeline.Events)
                if (JoinRoomEvent.Factory.TryCreateFrom(timelineEvent, roomId, out var joinRoomEvent))
                    joinedUserIds.Add(joinRoomEvent!.SenderUserId);

            return new MatrixRoom(roomId, MatrixRoomStatus.Left, joinedUserIds);
        }
    }
}