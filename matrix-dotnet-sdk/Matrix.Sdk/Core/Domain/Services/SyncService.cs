namespace Matrix.Sdk.Core.Domain.Services
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Infrastructure.Dto.Sync;
    using Infrastructure.Services;
    using MatrixRoom;
    using Microsoft.Extensions.Logging;

    public class SyncService : ISyncService
    {
        private readonly EventService _eventService;
        private readonly ILogger<SyncService> _logger;
        private readonly ConcurrentDictionary<string, MatrixRoom> _matrixRooms = new();

        private string? _accessToken;
        private CancellationToken _cancellationToken;
        private string _nextBatch;
        private Uri? _nodeAddress;
        private Timer? _pollingTimer;
        private ulong _timeout;

        public SyncService(EventService eventService, ILogger<SyncService> logger)
        {
            _eventService = eventService;
            _logger = logger;
            _timeout = Constants.FirstSyncTimout;
        }

        public MatrixRoom[] InvitedRooms =>
            _matrixRooms.Values.Where(x => x.Status == MatrixRoomStatus.Invited).ToArray();

        public MatrixRoom[] JoinedRooms =>
            _matrixRooms.Values.Where(x => x.Status == MatrixRoomStatus.Joined).ToArray();

        public MatrixRoom[] LeftRooms => _matrixRooms.Values.Where(x => x.Status == MatrixRoomStatus.Left).ToArray();

        public void Start(Uri nodeAddress, string accessToken, CancellationToken cancellationToken,
            Action<SyncBatch> onNewSyncBatch)
        {
            _nodeAddress = nodeAddress;
            _accessToken = accessToken;
            _cancellationToken = cancellationToken;

            _pollingTimer = new Timer(async _ => await PollAsync(onNewSyncBatch));
            _pollingTimer.Change(TimeSpan.Zero, TimeSpan.FromMilliseconds(-1));
        }

        public void Stop() => _pollingTimer!.Change(TimeSpan.Zero, TimeSpan.FromMilliseconds(-1));

        public void UpdateMatrixRoom(string roomId, MatrixRoom matrixRoom)
        {
            if (!_matrixRooms.TryGetValue(roomId, out MatrixRoom oldValue))
                _logger.LogInformation($"RoomId: {roomId}: could not get value");

            if (!_matrixRooms.TryUpdate(roomId, matrixRoom, oldValue))
                _logger.LogInformation($"RoomId: {roomId}: could not update value");
        }

        public MatrixRoom? GetMatrixRoom(string roomId) =>
            _matrixRooms.TryGetValue(roomId, out MatrixRoom matrixRoom) ? matrixRoom : null;

        private async Task PollAsync(Action<SyncBatch> onNewSyncBatch)
        {
            _pollingTimer!.Change(Timeout.Infinite, Timeout.Infinite);

            SyncResponse response = await _eventService.SyncAsync(_nodeAddress!, _accessToken!,
                timeout: _timeout,
                nextBatch: _nextBatch,
                cancellationToken: _cancellationToken);

            SyncBatch syncBatch = SyncBatch.Factory.CreateFromSync(response.NextBatch, response.Rooms);

            _nextBatch = syncBatch.NextBatch;
            _timeout = Constants.LaterSyncTimout;

            RefreshRooms(syncBatch.MatrixRooms);

            onNewSyncBatch(syncBatch);

            _pollingTimer.Change(TimeSpan.Zero, TimeSpan.FromMilliseconds(-1));
        }

        private void RefreshRooms(List<MatrixRoom> matrixRooms)
        {
            foreach (MatrixRoom room in matrixRooms)
                if (!_matrixRooms.TryGetValue(room.Id, out MatrixRoom retrievedRoom))
                {
                    _matrixRooms.TryAdd(room.Id, room);
                }
                else
                {
                    var updatedUserIds = retrievedRoom.JoinedUserIds.Concat(room.JoinedUserIds).ToList();
                    var updatedRoom = new MatrixRoom(retrievedRoom.Id, room.Status, updatedUserIds);

                    _matrixRooms.TryUpdate(room.Id, updatedRoom, retrievedRoom);
                }
        }
    }
}