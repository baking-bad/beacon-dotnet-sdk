namespace MatrixSdk
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using Domain;
    using Dto.Sync;

    public class MatrixManager
    {
        public readonly MatrixClientState state = new()
        {
            Id = Guid.NewGuid(),
            MatrixRooms = new ConcurrentDictionary<string, MatrixRoom>(),
            Timeout = 0,
            TransactionNumber = 0
        };
        //
        // public IDisposable Subscribe(IObserver<BaseEvent> observer) => throw new NotImplementedException();

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
            {
                if (JoinRoomEvent.Factory.TryCreateFrom(timelineEvent, roomId, out var joinRoomEvent))
                    joinedUserIds.Add(joinRoomEvent!.SenderUserId);
            }

            return new MatrixRoom(roomId, MatrixRoomStatus.Joined, joinedUserIds);
        }

        private MatrixRoom CreateInvite(string roomId, InvitedRoom invitedRoom)
        {
            var joinedUserIds = new List<string>();
            foreach (var timelineEvent in invitedRoom.InviteState.Events)
            {
                if (JoinRoomEvent.Factory.TryCreateFrom(timelineEvent, roomId, out var joinRoomEvent))
                    joinedUserIds.Add(joinRoomEvent!.SenderUserId);
            }
            
            return new MatrixRoom(roomId, MatrixRoomStatus.Joined, joinedUserIds);
        }

        private MatrixRoom CreateLeft(string roomId, LeftRoom leftRoom)
        {
            var joinedUserIds = new List<string>();
            foreach (var timelineEvent in leftRoom.Timeline.Events)
            {
                if (JoinRoomEvent.Factory.TryCreateFrom(timelineEvent, roomId, out var joinRoomEvent))
                    joinedUserIds.Add(joinRoomEvent!.SenderUserId);
            }

            return new MatrixRoom(roomId, MatrixRoomStatus.Left, joinedUserIds);
        }

        public void UpdateStateWith(List<MatrixRoom> matrixRooms)
        {
            foreach (var room in matrixRooms)
            {
                if (!state.MatrixRooms.TryGetValue(room.Id, out var retrievedRoom))
                    state.MatrixRooms.TryAdd(room.Id, room);
                else
                {
                    var updatedUserIds = retrievedRoom.JoinedUserIds.Concat(room.JoinedUserIds).ToList();
                    var updatedRoom = new MatrixRoom(retrievedRoom.Id, room.Status, updatedUserIds);

                    state.MatrixRooms.TryUpdate(room.Id, updatedRoom, retrievedRoom);
                }
            }
        }
    }
}