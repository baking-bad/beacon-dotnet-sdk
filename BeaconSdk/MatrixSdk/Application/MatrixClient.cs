namespace MatrixSdk.Application
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Domain;
    using Domain.Room;
    using Microsoft.Extensions.Logging;
    using Network;
    using Notifier;
    using Sodium;

    public class MatrixClient
    {
        private const int FirstSyncTimout = 0;
        private const int LaterSyncTimout = 30000;

        private readonly CancellationTokenSource _cts = new();
        private readonly ILogger<MatrixClient> _logger;
        private readonly INetworkService _networkService;
        private readonly ClientStateManager _stateManager;
        private Timer _pollingTimer = null!;

        // private string seed = "";

        public MatrixClient(
            ClientStateManager stateManager,
            INetworkService networkService,
            ILogger<MatrixClient> logger,
            MatrixEventNotifier<List<BaseRoomEvent>> matrixEventNotifier)
        {
            _stateManager = stateManager;
            _networkService = networkService;
            _logger = logger;

            MatrixEventNotifier = matrixEventNotifier;
        }

        public MatrixEventNotifier<List<BaseRoomEvent>> MatrixEventNotifier { get; }

        public string UserId => _stateManager.UserId!;

        //Todo: store on disk
        public MatrixRoom[] InvitedRooms => _stateManager.MatrixRooms.Values.Where(x => x.Status == MatrixRoomStatus.Invited).ToArray();

        public MatrixRoom[] JoinedRooms => _stateManager.MatrixRooms.Values.Where(x => x.Status == MatrixRoomStatus.Joined).ToArray();

        public MatrixRoom[] LeftRooms => _stateManager.MatrixRooms.Values.Where(x => x.Status == MatrixRoomStatus.Left).ToArray();

        public async Task StartAsync(KeyPair keyPair)
        {
            _logger.LogInformation("MatrixClient: Starting...");

            var response = await _networkService.LoginAsync(_cts.Token, keyPair);
            _stateManager.UpdateStateWith(response.UserId, response.AccessToken, FirstSyncTimout);

            _pollingTimer = new Timer(async _ => await PollAsync(_cts.Token));
            _pollingTimer.Change(TimeSpan.Zero, TimeSpan.FromMilliseconds(-1));

            _logger.LogInformation("MatrixClient: Ready");
        }

        private async Task PollAsync(CancellationToken cancellationToken)
        {
            _pollingTimer.Change(Timeout.Infinite, Timeout.Infinite);

            var response = await _networkService.SyncAsync(_stateManager.AccessToken, timeout: _stateManager.Timeout,
                nextBatch: _stateManager.NextBatch,
                cancellationToken: cancellationToken);

            var syncBatch = SyncBatch.Factory.CreateFromSync(response.NextBatch, response.Rooms);
            _stateManager.UpdateStateWith(syncBatch, syncBatch.NextBatch, LaterSyncTimout);

            MatrixEventNotifier.NotifyAll(syncBatch.MatrixRoomEvents);

            _pollingTimer.Change(TimeSpan.Zero, TimeSpan.FromMilliseconds(-1));
        }

        public void Stop()
        {
            _logger.LogInformation("MatrixClient: Stopping...");

            _cts.Cancel();
            _pollingTimer.Change(TimeSpan.Zero, TimeSpan.FromMilliseconds(-1));

            _logger.LogInformation("MatrixClient: Stopped");
        }

        public async Task<MatrixRoom> CreateTrustedPrivateRoomAsync(string[]? invitedUserIds = null) =>
            await _networkService.CreateTrustedPrivateRoomAsync(_stateManager, _cts.Token, invitedUserIds);

        public async Task<MatrixRoom> JoinTrustedPrivateRoomAsync(string roomId) =>
            await _networkService.JoinTrustedPrivateRoomAsync(_stateManager, _cts.Token, roomId);

        public async Task<string?> SendMessageAsync(string roomId, string message) =>
            await _networkService.SendMessageAsync(_stateManager, _cts.Token, roomId, message);

        public async Task<List<string>> GetJoinedRoomsIdsAsync() =>
            await _networkService.GetJoinedRoomsIdsAsync(_stateManager.AccessToken, _cts.Token);

        public async Task LeaveRoomAsync(string roomId) =>
            await _networkService.LeaveRoomAsync(_stateManager.AccessToken, _cts.Token, roomId);
    }
}