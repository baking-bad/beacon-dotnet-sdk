namespace Matrix.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Clients;
    using Core;
    using Core.Domain;
    using Core.Domain.Room;
    using Core.Infrastructure.Dto.Event;
    using Core.Infrastructure.Dto.Login;
    using Core.Infrastructure.Dto.Room.Create;
    using Core.Infrastructure.Dto.Room.Join;
    using Core.Infrastructure.Dto.Room.Joined;
    using Core.Infrastructure.Dto.Sync;
    using Microsoft.Extensions.Logging;
    using Sodium;

    public class MatrixClient : IMatrixClient
    {
        private readonly CancellationTokenSource _cts = new();
        private readonly EventClient _eventClient;
        private readonly ILogger<MatrixClient> _logger;
        private readonly RoomClient _roomClient;
        private readonly MatrixClientStateManager _stateManager;
        private readonly UserClient _userClient;
        private Timer _pollingTimer = null!;

        public MatrixClient(
            MatrixClientStateManager stateManager,
            ILogger<MatrixClient> logger,
            MatrixEventNotifier<List<BaseRoomEvent>> matrixEventNotifier,
            EventClient eventClient,
            RoomClient roomClient,
            UserClient userClient)
        {
            _stateManager = stateManager;
            _logger = logger;

            MatrixEventNotifier = matrixEventNotifier;
            _eventClient = eventClient;
            _roomClient = roomClient;
            _userClient = userClient;
        }

        public string UserId => _stateManager.UserId;

        public MatrixEventNotifier<List<BaseRoomEvent>> MatrixEventNotifier { get; }

        //Todo: store on disk
        public MatrixRoom[] InvitedRooms =>
            _stateManager.MatrixRooms.Values.Where(x => x.Status == MatrixRoomStatus.Invited).ToArray();

        public MatrixRoom[] JoinedRooms =>
            _stateManager.MatrixRooms.Values.Where(x => x.Status == MatrixRoomStatus.Joined).ToArray();

        public MatrixRoom[] LeftRooms =>
            _stateManager.MatrixRooms.Values.Where(x => x.Status == MatrixRoomStatus.Left).ToArray();

        public async Task StartAsync(KeyPair keyPair)
        {
            _logger.LogInformation("MatrixClient: Starting...");

            LoginResponse response = await _userClient.LoginAsync(keyPair, _cts.Token);
            _stateManager.UpdateStateWith(response.UserId, response.AccessToken, Constants.FirstSyncTimout);

            _pollingTimer = new Timer(async _ => await PollAsync());
            _pollingTimer.Change(TimeSpan.Zero, TimeSpan.FromMilliseconds(-1));

            _logger.LogInformation("MatrixClient: Ready");
        }

        public void Stop()
        {
            _logger.LogInformation("MatrixClient: Stopping...");

            _cts.Cancel();
            _pollingTimer.Change(TimeSpan.Zero, TimeSpan.FromMilliseconds(-1));

            _logger.LogInformation("MatrixClient: Stopped");
        }

        public async Task<MatrixRoom> CreateTrustedPrivateRoomAsync(string[] invitedUserIds)
        {
            CreateRoomResponse response =
                await _roomClient.CreateRoomAsync(_stateManager.AccessToken, invitedUserIds, _cts.Token);

            var matrixRoom = new MatrixRoom(response.RoomId, MatrixRoomStatus.Unknown);
            _stateManager.UpdateMatrixRoom(response.RoomId, matrixRoom);

            return matrixRoom;
        }

        public async Task<MatrixRoom> JoinTrustedPrivateRoomAsync(string roomId)
        {
            if (_stateManager.MatrixRooms.TryGetValue(roomId, out MatrixRoom matrixRoom))
                return matrixRoom;

            JoinRoomResponse response =
                await _roomClient.JoinRoomAsync(_stateManager.AccessToken, roomId, _cts.Token);

            matrixRoom = new MatrixRoom(response.RoomId, MatrixRoomStatus.Unknown);

            _stateManager.UpdateMatrixRoom(response.RoomId, matrixRoom);

            return matrixRoom;
        }

        public async Task<string> SendMessageAsync(string roomId, string message)
        {
            string transactionId = CreateTransactionId(_stateManager);
            EventResponse eventResponse = await _eventClient.SendMessageAsync(_stateManager.AccessToken, _cts.Token,
                roomId, transactionId, message);

            if (eventResponse.EventId == null)
                throw new NullReferenceException(nameof(eventResponse.EventId));

            return eventResponse.EventId;
        }

        public async Task<List<string>> GetJoinedRoomsIdsAsync()
        {
            JoinedRoomsResponse response = await _roomClient.GetJoinedRoomsAsync(_stateManager.AccessToken, _cts.Token);

            return response.JoinedRoomIds;
        }

        public async Task LeaveRoomAsync(string roomId) =>
            await _roomClient.LeaveRoomAsync(_stateManager.AccessToken, roomId, _cts.Token);

        private async Task PollAsync()
        {
            _pollingTimer.Change(Timeout.Infinite, Timeout.Infinite);

            SyncResponse response = await _eventClient.SyncAsync(_stateManager.AccessToken,
                timeout: _stateManager.Timeout,
                nextBatch: _stateManager.NextBatch,
                cancellationToken: _cts.Token);

            SyncBatch syncBatch = SyncBatch.Factory.CreateFromSync(response.NextBatch, response.Rooms);
            _stateManager.UpdateStateWith(syncBatch, syncBatch.NextBatch, Constants.LaterSyncTimout);

            MatrixEventNotifier.NotifyAll(syncBatch.MatrixRoomEvents);

            _pollingTimer.Change(TimeSpan.Zero, TimeSpan.FromMilliseconds(-1));
        }

        private static string CreateTransactionId(MatrixClientStateManager matrixClientStateManager)
        {
            long timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            ulong counter = matrixClientStateManager.TransactionNumber;

            matrixClientStateManager.TransactionNumber += 1;

            return $"m{timestamp}.{counter}";
        }
    }
}