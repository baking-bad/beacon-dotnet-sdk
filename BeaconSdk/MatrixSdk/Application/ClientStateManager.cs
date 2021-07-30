namespace MatrixSdk.Application
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using Domain;

    public class ClientStateManager
    {
        private const int FirstSyncTimout = 0;
        private const int LaterSyncTimout = 30000;

        public readonly MatrixClientState state = new()
        {
            Id = Guid.NewGuid(),
            MatrixRooms = new ConcurrentDictionary<string, MatrixRoom>(),
            Timeout = FirstSyncTimout,
            TransactionNumber = 0
        };

        private void UpdateStateWith(List<MatrixRoom> matrixRooms)
        {
            foreach (var room in matrixRooms)
                if (!state.MatrixRooms.TryGetValue(room.Id, out var retrievedRoom))
                {
                    state.MatrixRooms.TryAdd(room.Id, room);
                }
                else
                {
                    var updatedUserIds = retrievedRoom.JoinedUserIds.Concat(room.JoinedUserIds).ToList();
                    var updatedRoom = new MatrixRoom(retrievedRoom.Id, room.Status, updatedUserIds);

                    state.MatrixRooms.TryUpdate(room.Id, updatedRoom, retrievedRoom);
                }
        }

        public void OnSuccessSync(SyncBatch syncBatch, string nextBatch)
        {
            state.Timeout = LaterSyncTimout;
            state.NextBatch = nextBatch;
            UpdateStateWith(syncBatch.MatrixRooms);
        }

        public void UpdateStateWith(string userId, string accessToken)
        {
            state.UserId = userId;
            state.AccessToken = accessToken;
        }
    }
}