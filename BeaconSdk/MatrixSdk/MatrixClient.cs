﻿namespace MatrixSdk
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Dto.Sync.Event;
    using Extensions;
    using Microsoft.Extensions.Logging;
    using Services;

    public struct Message
    {
        public string SenderId { get; set; }

        public string Text { get; set; }
    }

    public class MessageObserver : IObserver<BaseEvent>
    {
        public void OnCompleted()
        {
            throw new NotImplementedException();
        }
        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }
        public void OnNext(BaseEvent value)
        {
            if (value.EventType != EventType.Message) return;
        }
    }

    public class MatrixClient
    {
        private readonly CancellationTokenSource cancellationTokenSource = new();
        private readonly EventService eventService;
        private readonly MatrixManager manager;
        private readonly ILogger<MatrixClient> logger;
        private readonly RoomService roomService;


        private readonly UserService userService;

        private Timer pollingTimer;

        // public MatrixRoom[] InvitedRooms => state.MatrixRooms.Values.Where(x => x.Status == MatrixRoomStatus.Invited).ToArray();

        private string Seed;

        public MatrixClient(ILogger<MatrixClient> logger, UserService userService, RoomService roomService,
            EventService eventService, MatrixManager manager)
        {
            this.logger = logger;
            this.userService = userService;
            this.roomService = roomService;
            this.eventService = eventService;
            this.manager = manager;
        }

        public string UserId => manager.state.UserId!;

        //Todo: store on disk
        public MatrixRoom[] InvitedRooms => manager.state.MatrixRooms.Values.Where(x => x.Status == MatrixRoomStatus.Invited).ToArray();
        public MatrixRoom[] JoinedRooms => manager.state.MatrixRooms.Values.Where(x => x.Status == MatrixRoomStatus.Joined).ToArray();
        public async Task StartAsync(string seed)
        {
            Seed = seed;
            logger.LogInformation($"{nameof(MatrixClient)}: Starting...");

            var response = await userService!.LoginAsync(seed, cancellationTokenSource.Token);
            manager.state.UserId = response.UserId;
            manager.state.AccessToken = response.AccessToken;

            pollingTimer = new Timer(async _ => await PollAsync(cancellationTokenSource.Token));
            pollingTimer.Change(TimeSpan.Zero, TimeSpan.FromMilliseconds(-1));

            logger.LogInformation($"{nameof(MatrixClient)}: Ready.");
        }

        private async Task PollAsync(CancellationToken cancellationToken)
        {
            var seed = Seed;
            pollingTimer.Change(Timeout.Infinite, Timeout.Infinite);

            ThrowIfAccessTokenIsEmpty();

            var response = await eventService.SyncAsync(manager.state.AccessToken!, timeout: manager.state.Timeout, nextBatch: manager.state.NextBatch!,
                cancellationToken: cancellationToken);

            manager.state.Timeout = 30000;
            manager.state.NextBatch = response.NextBatch;

            var matrixRooms = manager.GetMatrixRoomsFromSync(response.Rooms);
            manager.UpdateStateWith(matrixRooms);
            
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


            if (seed == "777")
            {
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

            var response = await roomService.CreateRoomAsync(manager.state.AccessToken!, invitedUserIds, cancellationTokenSource.Token);
            var matrixRoom = new MatrixRoom(response.RoomId, MatrixRoomStatus.Unknown);
            manager.state.MatrixRooms[response.RoomId] = matrixRoom;

            return matrixRoom;
        }

        public async Task<MatrixRoom> JoinTrustedPrivateRoomAsync(string roomId)
        {
            if (manager.state.MatrixRooms.TryGetValue(roomId, out var matrixRoom))
                return matrixRoom;

            ThrowIfAccessTokenIsEmpty();

            var response = await roomService.JoinRoomAsync(manager.state.AccessToken!, roomId, CancellationToken.None);
            matrixRoom = new MatrixRoom(response.RoomId, MatrixRoomStatus.Unknown);
            manager.state.MatrixRooms[response.RoomId] = matrixRoom;

            return matrixRoom;
        }

        public async Task SendMessageAsync(string roomId, string message)
        {
            ThrowIfAccessTokenIsEmpty();

            var transactionId = CreateTransactionId();
            var result = await eventService.SendMessageAsync(manager.state.AccessToken!, roomId, transactionId, message);
            var id = result.EventId;
        }

        public async Task<List<string>> GetJoinedRoomsIdsAsync()
        {
            ThrowIfAccessTokenIsEmpty();

            var response = await roomService.GetJoinedRoomsAsync(manager.state.AccessToken!, cancellationTokenSource.Token);

            return response.JoinedRoomIds;
        }

        public async Task LeaveRoomAsync(string roomId)
        {
            ThrowIfAccessTokenIsEmpty();

            await roomService.LeaveRoomAsync(manager.state.AccessToken!, roomId, cancellationTokenSource.Token);
        }

        private void ThrowIfAccessTokenIsEmpty()
        {
            if (manager.state.AccessToken == null)
                throw new InvalidOperationException("No access token has been provided.");
        }

        /*
         * Create a transaction ID
         */
        private string CreateTransactionId()
        {
            var timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            var counter = manager.state.TransactionNumber;

            manager.state.TransactionNumber += 1;

            return $"m{timestamp}.{counter}";
        }
    }
}


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