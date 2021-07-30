namespace MatrixSdk.Application
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Domain;
    using Domain.Room;
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

        public string UserId => clientStateManager.state.UserId!;

        //Todo: store on disk
        public MatrixRoom[] InvitedRooms => clientStateManager.state.MatrixRooms.Values.Where(x => x.Status == MatrixRoomStatus.Invited).ToArray();

        public MatrixRoom[] JoinedRooms => clientStateManager.state.MatrixRooms.Values.Where(x => x.Status == MatrixRoomStatus.Joined).ToArray();

        public MatrixRoom[] LeftRooms => clientStateManager.state.MatrixRooms.Values.Where(x => x.Status == MatrixRoomStatus.Left).ToArray();

        public async Task StartAsync(string seed)
        {
            this.seed = seed;
            logger.LogInformation($"{nameof(MatrixClient)}: Starting...");

            var response = await userService!.LoginAsync(seed, cancellationTokenSource.Token);

            clientStateManager.state.UserId = response.UserId;
            clientStateManager.state.AccessToken = response.AccessToken;

            pollingTimer = new Timer(async _ => await PollAsync(cancellationTokenSource.Token));
            pollingTimer.Change(TimeSpan.FromSeconds(clientStateManager.state.Timeout), TimeSpan.FromMilliseconds(-1));

            logger.LogInformation($"{nameof(MatrixClient)}: Ready.");
        }

        private async Task PollAsync(CancellationToken cancellationToken)
        {
            pollingTimer.Change(Timeout.Infinite, Timeout.Infinite);

            ThrowIfAccessTokenIsEmpty();

            var response = await eventService.SyncAsync(clientStateManager.state.AccessToken!, timeout: clientStateManager.state.Timeout,
                nextBatch: clientStateManager.state.NextBatch!,
                cancellationToken: cancellationToken);

            var syncBatch = SyncBatch.Factory.CreateFromSync(response.NextBatch, response.Rooms);
            
            clientStateManager.OnSuccessSync(syncBatch, syncBatch.NextBatch);
            NotifyWithTextMessageEvents(syncBatch.MatrixRoomEvents);

            if (seed == "77777")
            {
                var t = clientStateManager.state.MatrixRooms;
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

        private void NotifyWithTextMessageEvents(List<BaseRoomEvent> baseRoomEvents)
        {
            foreach (var matrixRoomEvent in baseRoomEvents)
            {
                var textMessageEvent = matrixRoomEvent as TextMessageEvent;
                if (textMessageEvent != null)
                    TextMessageNotifier.NotifyAll(textMessageEvent);
            }
        }

        public async Task<MatrixRoom> CreateTrustedPrivateRoomAsync(string[]? invitedUserIds = null)
        {
            ThrowIfAccessTokenIsEmpty();

            var response = await roomService.CreateRoomAsync(clientStateManager.state.AccessToken!, invitedUserIds, cancellationTokenSource.Token);
            var matrixRoom = new MatrixRoom(response.RoomId, MatrixRoomStatus.Unknown);
            clientStateManager.state.MatrixRooms[response.RoomId] = matrixRoom;

            return matrixRoom;
        }

        public async Task<MatrixRoom> JoinTrustedPrivateRoomAsync(string roomId)
        {
            if (clientStateManager.state.MatrixRooms.TryGetValue(roomId, out var matrixRoom))
                return matrixRoom;

            ThrowIfAccessTokenIsEmpty();

            var response = await roomService.JoinRoomAsync(clientStateManager.state.AccessToken!, roomId, CancellationToken.None);
            matrixRoom = new MatrixRoom(response.RoomId, MatrixRoomStatus.Unknown);
            clientStateManager.state.MatrixRooms[response.RoomId] = matrixRoom;

            return matrixRoom;
        }

        public async Task SendMessageAsync(string roomId, string message)
        {
            ThrowIfAccessTokenIsEmpty();

            var transactionId = CreateTransactionId();
            var result = await eventService.SendMessageAsync(clientStateManager.state.AccessToken!, roomId, transactionId, message);
            var id = result.EventId;
        }

        public async Task<List<string>> GetJoinedRoomsIdsAsync()
        {
            ThrowIfAccessTokenIsEmpty();

            var response = await roomService.GetJoinedRoomsAsync(clientStateManager.state.AccessToken!, cancellationTokenSource.Token);

            return response.JoinedRoomIds;
        }

        public async Task LeaveRoomAsync(string roomId)
        {
            ThrowIfAccessTokenIsEmpty();

            await roomService.LeaveRoomAsync(clientStateManager.state.AccessToken!, roomId, cancellationTokenSource.Token);
        }

        private void ThrowIfAccessTokenIsEmpty()
        {
            if (clientStateManager.state.AccessToken == null)
                throw new InvalidOperationException("No access token has been provided.");
        }

        /*
         * Create a transaction ID
         */
        private string CreateTransactionId()
        {
            var timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            var counter = clientStateManager.state.TransactionNumber;

            clientStateManager.state.TransactionNumber += 1;

            return $"m{timestamp}.{counter}";
        }
    }
}

// logger.LogInformation($"Id: {UserId.TruncateLongString(5)}, Invite: {response.Rooms.Invite.Count}");
// logger.LogInformation($"Id: {UserId.TruncateLongString(5)}, Join: {response.Rooms.Join.Count}");
// logger.LogInformation($"Id: {UserId.TruncateLongString(5)}, Leave: {response.Rooms.Leave.Count}");

// if (response.Rooms.Join.Count > 0)
// {
//     var joinRooms = response.Rooms.Join;
//     if (joinRooms.Count > 0)
//         foreach (var (roomId, joinedRoom) in joinRooms)
//         {
//             logger.LogInformation($"Id: {UserId.TruncateLongString(5)}, State.Events.Count: {joinedRoom.State.Events.Count}");
//             logger.LogInformation($"Id: {UserId.TruncateLongString(5)}, joinedRoom.Timeline.Events.Count: {joinedRoom.Timeline.Events.Count}");
//         }
// }

// if (response.Rooms.Invite.Count > 0)
// {
//     var joined = response.Rooms.Invite;
//
//     foreach (var (roomId, joinedRoom) in joined)
//     {
//         var joinedRoomEvents = joinedRoom.InviteState.Events;
//         foreach (var joinedRoomEvent in joinedRoomEvents)
//         {
//             var type = joinedRoomEvent.RoomEventType;
//             switch (type)
//             {
//                 case RoomEventType.Create:
//                     var roomCreateContent = joinedRoomEvent.Content.ToObject<RoomCreateContent>();
//                     break;
//                 case RoomEventType.JoinRules:
//                     var roomJoinRulesContent = joinedRoomEvent.Content.ToObject<RoomJoinRulesContent>();
//                     break;
//                 case RoomEventType.Member:
//                     var roomMemberContent = joinedRoomEvent.Content.ToObject<RoomMemberContent>();
//                     break;
//                 default:
//                     throw new ArgumentOutOfRangeException();
//             }
//         }
//     }
// }