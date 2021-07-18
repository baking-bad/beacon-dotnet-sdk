namespace MatrixSdk.Application
{
    using System.Collections.Generic;
    using System.Linq;
    using Domain;
    using Domain.Room;
    using Infrastructure.Dto.Sync;

    public class MatrixRoomSyncService
    {
        public List<MatrixRoom> GetMatrixRoomsFromSync(Rooms rooms)
        {
            var joinedMatrixRooms = rooms.Join.Select(pair => CreateJoined(pair.Key, pair.Value)).ToList();
            var invitedMatrixRooms = rooms.Invite.Select(pair => CreateInvite(pair.Key, pair.Value)).ToList();
            var leftMatrixRooms = rooms.Leave.Select(pair => CreateLeft(pair.Key, pair.Value)).ToList();

            return joinedMatrixRooms.Concat(invitedMatrixRooms).Concat(leftMatrixRooms).ToList();
        }

        private MatrixRoom CreateJoined(string roomId, JoinedRoom joinedRoom)
        {
            var joinedUserIds = new List<string>();
            foreach (var timelineEvent in joinedRoom.Timeline.Events)
                if (JoinRoomEvent.Factory.TryCreateFrom(timelineEvent, roomId, out var joinRoomEvent))
                    joinedUserIds.Add(joinRoomEvent!.SenderUserId);

            return new MatrixRoom(roomId, MatrixRoomStatus.Joined, joinedUserIds);
        }

        private MatrixRoom CreateInvite(string roomId, InvitedRoom invitedRoom)
        {
            var joinedUserIds = new List<string>();
            foreach (var timelineEvent in invitedRoom.InviteState.Events)
                if (JoinRoomEvent.Factory.TryCreateFrom(timelineEvent, roomId, out var joinRoomEvent))
                    joinedUserIds.Add(joinRoomEvent!.SenderUserId);

            return new MatrixRoom(roomId, MatrixRoomStatus.Joined, joinedUserIds);
        }

        private MatrixRoom CreateLeft(string roomId, LeftRoom leftRoom)
        {
            var joinedUserIds = new List<string>();
            foreach (var timelineEvent in leftRoom.Timeline.Events)
                if (JoinRoomEvent.Factory.TryCreateFrom(timelineEvent, roomId, out var joinRoomEvent))
                    joinedUserIds.Add(joinRoomEvent!.SenderUserId);

            return new MatrixRoom(roomId, MatrixRoomStatus.Left, joinedUserIds);
        }
    }
}