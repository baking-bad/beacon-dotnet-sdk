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

        private readonly CancellationTokenSource cts = new();
        private readonly ILogger<MatrixClient> logger;
        private readonly INetworkService networkService;
        private readonly ClientStateManager stateManager;
        private Timer pollingTimer = null!;

        // private string seed = "";

        public MatrixClient(
            ClientStateManager stateManager,
            INetworkService networkService,
            ILogger<MatrixClient> logger, 
            MatrixEventNotifier<List<BaseRoomEvent>> matrixEventNotifier)
        {
            this.stateManager = stateManager;
            this.networkService = networkService;
            this.logger = logger;
            
            MatrixEventNotifier = matrixEventNotifier;
        }

        public MatrixEventNotifier<List<BaseRoomEvent>> MatrixEventNotifier { get; }
        
        public string UserId => stateManager.UserId!;

        //Todo: store on disk
        public MatrixRoom[] InvitedRooms => stateManager.MatrixRooms.Values.Where(x => x.Status == MatrixRoomStatus.Invited).ToArray();

        public MatrixRoom[] JoinedRooms => stateManager.MatrixRooms.Values.Where(x => x.Status == MatrixRoomStatus.Joined).ToArray();

        public MatrixRoom[] LeftRooms => stateManager.MatrixRooms.Values.Where(x => x.Status == MatrixRoomStatus.Left).ToArray();

        public async Task StartAsync(KeyPair keyPair)
        {
            logger.LogInformation($"{nameof(MatrixClient)}: Starting...");

            var response = await networkService.LoginAsync(cts.Token, keyPair);
            stateManager.UpdateStateWith(response.UserId, response.AccessToken, FirstSyncTimout);

            pollingTimer = new Timer(async _ => await PollAsync(cts.Token));
            pollingTimer.Change(TimeSpan.Zero, TimeSpan.FromMilliseconds(-1));

            logger.LogInformation($"{nameof(MatrixClient)}: Ready.");
        }

        private async Task PollAsync(CancellationToken cancellationToken)
        {
            pollingTimer.Change(Timeout.Infinite, Timeout.Infinite);

            var response = await networkService.SyncAsync(stateManager.AccessToken, timeout: stateManager.Timeout,
                nextBatch: stateManager.NextBatch,
                cancellationToken: cancellationToken);

            var syncBatch = SyncBatch.Factory.CreateFromSync(response.NextBatch, response.Rooms);
            stateManager.UpdateStateWith(syncBatch, syncBatch.NextBatch, LaterSyncTimout);
            
            MatrixEventNotifier.NotifyAll(syncBatch.MatrixRoomEvents);

            pollingTimer.Change(TimeSpan.Zero, TimeSpan.FromMilliseconds(-1));
        }

        public void Stop()
        {
            logger.LogInformation($"{nameof(MatrixClient)}: Stopping...");

            cts.Cancel();
            pollingTimer.Change(TimeSpan.Zero, TimeSpan.FromMilliseconds(-1));

            logger.LogInformation($"{nameof(MatrixClient)}: Stopped.");
        }

        public async Task<MatrixRoom> CreateTrustedPrivateRoomAsync(string[]? invitedUserIds = null) =>
            await networkService.CreateTrustedPrivateRoomAsync(stateManager, cts.Token, invitedUserIds);

        public async Task<MatrixRoom> JoinTrustedPrivateRoomAsync(string roomId) =>
            await networkService.JoinTrustedPrivateRoomAsync(stateManager, cts.Token, roomId);

        public async Task SendMessageAsync(string roomId, string message) =>
            await networkService.SendMessageAsync(stateManager, cts.Token, roomId, message);

        public async Task<List<string>> GetJoinedRoomsIdsAsync() =>
            await networkService.GetJoinedRoomsIdsAsync(stateManager.AccessToken, cts.Token);

        public async Task LeaveRoomAsync(string roomId) =>
            await networkService.LeaveRoomAsync(stateManager.AccessToken, cts.Token, roomId);
    }
}

// if (seed == "0008777")
// {
//     var t = clientStateManager.MatrixRooms;
// }