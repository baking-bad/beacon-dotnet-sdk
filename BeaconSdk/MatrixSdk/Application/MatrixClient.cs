namespace MatrixSdk.Application
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Domain;
    using Infrastructure.Services;
    using Microsoft.Extensions.Logging;

    public class MatrixClient
    {
        private readonly CancellationTokenSource cancellationTokenSource = new();
        private readonly ClientStateManager clientStateManager;
        private readonly EventService eventService;
        private readonly ILogger<MatrixClient> logger;
        private readonly RoomService roomService;
        private readonly UserService userService;

        private Timer pollingTimer = null!;

        private string seed = "";

        public MatrixClient(
            ILogger<MatrixClient> logger,
            ClientStateManager clientStateManager,
            UserService userService,
            RoomService roomService,
            EventService eventService, TextMessageNotifier textMessageNotifier)
        {
            this.logger = logger;
            this.clientStateManager = clientStateManager;
            this.userService = userService;
            this.roomService = roomService;
            this.eventService = eventService;
            TextMessageNotifier = textMessageNotifier;
        }
        public TextMessageNotifier TextMessageNotifier { get; }

        public string UserId => clientStateManager.UserId!;

        //Todo: store on disk
        public MatrixRoom[] InvitedRooms => clientStateManager.MatrixRooms.Values.Where(x => x.Status == MatrixRoomStatus.Invited).ToArray();

        public MatrixRoom[] JoinedRooms => clientStateManager.MatrixRooms.Values.Where(x => x.Status == MatrixRoomStatus.Joined).ToArray();

        public MatrixRoom[] LeftRooms => clientStateManager.MatrixRooms.Values.Where(x => x.Status == MatrixRoomStatus.Left).ToArray();

        public async Task StartAsync(string seed) // seed need for debugging
        {
            logger.LogInformation($"{nameof(MatrixClient)}: Starting...");
            this.seed = seed;

            var response = await userService!.LoginAsync(seed, cancellationTokenSource.Token);
            clientStateManager.UpdateStateWith(response.UserId, response.AccessToken);

            pollingTimer = new Timer(async _ => await PollAsync(cancellationTokenSource.Token));
            pollingTimer.Change(TimeSpan.FromSeconds(clientStateManager.Timeout), TimeSpan.FromMilliseconds(-1));

            logger.LogInformation($"{nameof(MatrixClient)}: Ready.");
        }

        private async Task PollAsync(CancellationToken cancellationToken)
        {
            pollingTimer.Change(Timeout.Infinite, Timeout.Infinite);

            ThrowIfAccessTokenIsEmpty();

            var response = await eventService.SyncAsync(clientStateManager.AccessToken, timeout: clientStateManager.Timeout,
                nextBatch: clientStateManager.NextBatch,
                cancellationToken: cancellationToken);

            var syncBatch = SyncBatch.Factory.CreateFromSync(response.NextBatch, response.Rooms);

            clientStateManager.UpdateStateWith(syncBatch, syncBatch.NextBatch);
            TextMessageNotifier.NotifyAll(syncBatch.MatrixRoomEvents);

            if (seed == "0008777")
            {
                var t = clientStateManager.MatrixRooms;
            }

            pollingTimer.Change(TimeSpan.Zero, TimeSpan.FromMilliseconds(-1));
        }

        public void Stop()
        {
            logger.LogInformation($"{nameof(MatrixClient)}: Stopping...");

            cancellationTokenSource.Cancel();
            pollingTimer.Change(TimeSpan.Zero, TimeSpan.FromMilliseconds(-1));

            logger.LogInformation($"{nameof(MatrixClient)}: Stopped.");
        }

        public async Task<MatrixRoom> CreateTrustedPrivateRoomAsync(string[]? invitedUserIds = null)
        {
            ThrowIfAccessTokenIsEmpty();

            var response = await roomService.CreateRoomAsync(clientStateManager.AccessToken!, invitedUserIds, cancellationTokenSource.Token);
            var matrixRoom = new MatrixRoom(response.RoomId, MatrixRoomStatus.Unknown);
            clientStateManager.UpdateMatrixRoom(response.RoomId, matrixRoom);

            return matrixRoom;
        }

        public async Task<MatrixRoom> JoinTrustedPrivateRoomAsync(string roomId)
        {
            if (clientStateManager.MatrixRooms.TryGetValue(roomId, out var matrixRoom))
                return matrixRoom;

            ThrowIfAccessTokenIsEmpty();

            var response = await roomService.JoinRoomAsync(clientStateManager.AccessToken!, roomId, CancellationToken.None);
            matrixRoom = new MatrixRoom(response.RoomId, MatrixRoomStatus.Unknown);
            clientStateManager.UpdateMatrixRoom(response.RoomId, matrixRoom);

            return matrixRoom;
        }

        public async Task SendMessageAsync(string roomId, string message)
        {
            ThrowIfAccessTokenIsEmpty();

            var transactionId = CreateTransactionId();
            var result = await eventService.SendMessageAsync(clientStateManager.AccessToken!, roomId, transactionId, message);
            var id = result.EventId;
        }

        private string CreateTransactionId()
        {
            var timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            var counter = clientStateManager.TransactionNumber;

            clientStateManager.TransactionNumber += 1;

            return $"m{timestamp}.{counter}";
        }

        public async Task<List<string>> GetJoinedRoomsIdsAsync()
        {
            ThrowIfAccessTokenIsEmpty();

            var response = await roomService.GetJoinedRoomsAsync(clientStateManager.AccessToken!, cancellationTokenSource.Token);

            return response.JoinedRoomIds;
        }

        public async Task LeaveRoomAsync(string roomId)
        {
            ThrowIfAccessTokenIsEmpty();

            await roomService.LeaveRoomAsync(clientStateManager.AccessToken!, roomId, cancellationTokenSource.Token);
        }

        private void ThrowIfAccessTokenIsEmpty()
        {
            if (clientStateManager.AccessToken == null)
                throw new InvalidOperationException("No access token has been provided.");
        }
    }
}