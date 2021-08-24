namespace MatrixSdk.Application
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using Domain;
    using Microsoft.Extensions.Logging;

    public class ClientStateManager
    {
        private readonly ILogger<ClientStateManager> logger;

        private readonly MatrixClientState state = new()
        {
            Id = Guid.NewGuid(),
            MatrixRooms = new ConcurrentDictionary<string, MatrixRoom>(),
            Timeout = 0,
            TransactionNumber = 0
        };

        public ClientStateManager(ILogger<ClientStateManager> logger)
        {
            this.logger = logger;
        }

        public string AccessToken => state.AccessToken!;

        public ulong Timeout => state.Timeout;

        public string UserId => state.UserId!;

        public string NextBatch => state.NextBatch!;

        public ConcurrentDictionary<string, MatrixRoom> MatrixRooms => state.MatrixRooms;

        public ulong TransactionNumber
        {
            get => state.TransactionNumber;
            set => state.TransactionNumber = value;
        }

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

        public void UpdateStateWith(SyncBatch syncBatch, string nextBatch, ulong timeout)
        {
            state.NextBatch = nextBatch;
            UpdateStateWith(syncBatch.MatrixRooms);

            state.Timeout = timeout;
        }

        public void UpdateStateWith(string userId, string accessToken, ulong timeout)
        {
            state.UserId = userId;
            state.AccessToken = accessToken;

            state.Timeout = timeout;
        }

        public void UpdateMatrixRoom(string roomId, MatrixRoom matrixRoom)
        {
            if (!state.MatrixRooms.TryGetValue(roomId, out var oldValue))
                logger.LogInformation($"RoomId: {roomId}: could not get value");

            if (!state.MatrixRooms.TryUpdate(roomId, matrixRoom, oldValue))
                logger.LogInformation($"RoomId: {roomId}: could not update value");
        }
    }
}