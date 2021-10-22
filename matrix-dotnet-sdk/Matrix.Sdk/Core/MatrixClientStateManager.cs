namespace Matrix.Sdk.Core
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using Domain;
    using Microsoft.Extensions.Logging;

    public class MatrixClientStateManager
    {
        private readonly ILogger<MatrixClientStateManager> _logger;

        private readonly MatrixClientState _state = new()
        {
            Id = Guid.NewGuid(),
            MatrixRooms = new ConcurrentDictionary<string, MatrixRoom>(),
            Timeout = 0,
            TransactionNumber = 0
        };

        public MatrixClientStateManager(ILogger<MatrixClientStateManager> logger)
        {
            _logger = logger;
        }

        public string AccessToken => _state.AccessToken ??
                                     throw new InvalidOperationException("No access token has been provided.");

        public ulong Timeout => _state.Timeout;

        // public string UserId => _state.UserId!;

        public string NextBatch => _state.NextBatch!;

        public ConcurrentDictionary<string, MatrixRoom> MatrixRooms => _state.MatrixRooms;

        public ulong TransactionNumber
        {
            get => _state.TransactionNumber;
            set => _state.TransactionNumber = value;
        }

        private void UpdateStateWith(List<MatrixRoom> matrixRooms)
        {
            foreach (MatrixRoom room in matrixRooms)
                if (!_state.MatrixRooms.TryGetValue(room.Id, out MatrixRoom retrievedRoom))
                {
                    _state.MatrixRooms.TryAdd(room.Id, room);
                }
                else
                {
                    var updatedUserIds = retrievedRoom.JoinedUserIds.Concat(room.JoinedUserIds).ToList();
                    var updatedRoom = new MatrixRoom(retrievedRoom.Id, room.Status, updatedUserIds);

                    _state.MatrixRooms.TryUpdate(room.Id, updatedRoom, retrievedRoom);
                }
        }

        public void UpdateStateWith(SyncBatch syncBatch, string nextBatch, ulong timeout)
        {
            _state.NextBatch = nextBatch;
            UpdateStateWith(syncBatch.MatrixRooms);

            _state.Timeout = timeout;
        }

        public void UpdateStateWith(string userId, string accessToken, ulong timeout)
        {
            _state.UserId = userId;
            _state.AccessToken = accessToken;

            _state.Timeout = timeout;
        }

        public void UpdateMatrixRoom(string roomId, MatrixRoom matrixRoom)
        {
            if (!_state.MatrixRooms.TryGetValue(roomId, out MatrixRoom oldValue))
                _logger.LogInformation($"RoomId: {roomId}: could not get value");

            if (!_state.MatrixRooms.TryUpdate(roomId, matrixRoom, oldValue))
                _logger.LogInformation($"RoomId: {roomId}: could not update value");
        }
    }
}