namespace MatrixSdk.Application
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using Domain;

    public class MatrixClientStateManager
    {
        public readonly MatrixClientState state = new()
        {
            Id = Guid.NewGuid(),
            MatrixRooms = new ConcurrentDictionary<string, MatrixRoom>(),
            Timeout = 0,
            TransactionNumber = 0
        };

        public void UpdateStateWith(List<MatrixRoom> matrixRooms)
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
    }
}