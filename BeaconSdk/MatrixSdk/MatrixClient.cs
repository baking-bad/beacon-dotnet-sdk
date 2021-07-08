﻿namespace MatrixSdk
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Extensions;
    using Microsoft.Extensions.Logging;
    using Services;


    public interface IEventListener
    {
    }

    public class MatrixClient
    {
        private readonly CancellationTokenSource cancellationTokenSource = new();
        private readonly EventService eventService;

        private readonly ILogger<MatrixClient> logger;
        private readonly RoomService roomService;

        private readonly MatrixClientState state = new()
        {
            Id = Guid.NewGuid(),
            MatrixRooms = new ConcurrentDictionary<string, MatrixRoom>(),
            Timeout = 0,
            TransactionNumber = 0
        };

        private readonly UserService userService;

        private Timer pollingTimer;

        public MatrixClient(ILogger<MatrixClient> logger, UserService userService, RoomService roomService,
            EventService eventService)
        {
            this.logger = logger;
            this.userService = userService;
            this.roomService = roomService;
            this.eventService = eventService;
        }

        public string UserId => state.UserId!;

        //Todo: store on disk
        public MatrixRoom[] InvitedRooms => state.MatrixRooms.Values.Where(x => x.Status == MatrixRoomStatus.Invited).ToArray();
        public MatrixRoom[] JoinedRooms => state.MatrixRooms.Values.Where(x => x.Status == MatrixRoomStatus.Joined).ToArray();

        // public MatrixRoom[] InvitedRooms => state.MatrixRooms.Values.Where(x => x.Status == MatrixRoomStatus.Invited).ToArray();


        public async Task StartAsync(string seed)
        {
            logger.LogInformation($"{nameof(MatrixClient)}: Starting...");

            var response = await userService!.LoginAsync(seed, cancellationTokenSource.Token);
            state.UserId = response.UserId;
            state.AccessToken = response.AccessToken;

            pollingTimer = new Timer(async _ => await PollAsync(cancellationTokenSource.Token));
            pollingTimer.Change(TimeSpan.Zero, TimeSpan.FromMilliseconds(-1));

            logger.LogInformation($"{nameof(MatrixClient)}: Ready.");
        }

        private async Task PollAsync(CancellationToken cancellationToken)
        {
            pollingTimer.Change(Timeout.Infinite, Timeout.Infinite);

            ThrowIfAccessTokenIsEmpty();

            var response = await eventService.SyncAsync(state.AccessToken!, timeout: state.Timeout, nextBatch: state.NextBatch!,
                cancellationToken: cancellationToken);

            state.Timeout = 30000;
            state.NextBatch = response.NextBatch;

            logger.LogInformation($"Id: {UserId.TruncateLongString(5)}, Invite: {response.Rooms.Invite.Count}");
            logger.LogInformation($"Id: {UserId.TruncateLongString(5)}, Join: {response.Rooms.Join.Count}");
            logger.LogInformation($"Id: {UserId.TruncateLongString(5)}, Leave: {response.Rooms.Leave.Count}");

            if (response.Rooms.Invite.Count > 0)
                foreach (var (key, value) in response.Rooms.Invite)
                {
                    var roomId = key;
                }
            // var roomId = response.Rooms.Invite[0];
            // var joined = response.Rooms.Invite;
            //
            // foreach (var (roomId, joinedRoom) in joined)
            // {
            //     var joinedRoomEvents = joinedRoom.InviteState.Events;
            //     foreach (var joinedRoomEvent in joinedRoomEvents)
            //     {
            //         var type = joinedRoomEvent.RoomEventType;
            //         switch (type)
            //         {
            //             case RoomEventType.Create:
            //                 var roomCreateContent = joinedRoomEvent.Content.ToObject<RoomCreateContent>();
            //                 break;
            //             case RoomEventType.JoinRules:
            //                 var roomJoinRulesContent = joinedRoomEvent.Content.ToObject<RoomJoinRulesContent>();
            //                 break;
            //             case RoomEventType.Member:
            //                 var roomMemberContent = joinedRoomEvent.Content.ToObject<RoomMemberContent>();
            //                 break;
            //             default:
            //                 throw new ArgumentOutOfRangeException();
            //         }
            //     }
            // }

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

            var response = await roomService.CreateRoomAsync(state.AccessToken!, invitedUserIds, cancellationTokenSource.Token);
            var matrixRoom = new MatrixRoom(response.RoomId, MatrixRoomStatus.Unknown);
            state.MatrixRooms[response.RoomId] = matrixRoom;

            return matrixRoom;
        }

        public async Task<MatrixRoom> JoinTrustedPrivateRoomAsync(string roomId)
        {
            if (state.MatrixRooms.TryGetValue(roomId, out var matrixRoom))
                return matrixRoom;

            ThrowIfAccessTokenIsEmpty();

            var response = await roomService.JoinRoomAsync(state.AccessToken!, roomId, CancellationToken.None);
            matrixRoom = new MatrixRoom(response.RoomId, MatrixRoomStatus.Unknown);
            state.MatrixRooms[response.RoomId] = matrixRoom;

            return matrixRoom;
        }

        public async Task SendMessageAsync(string roomId, string message)
        {
            ThrowIfAccessTokenIsEmpty();

            var transactionId = CreateTransactionId();
            var result = await eventService.SendMessageAsync(state.AccessToken, roomId, transactionId, message);
            var id = result.EventId;
        }

        public async Task<List<string>> GetJoinedRoomsIdsAsync()
        {
            ThrowIfAccessTokenIsEmpty();

            var response = await roomService.GetJoinedRoomsAsync(state.AccessToken!, cancellationTokenSource.Token);

            return response.JoinedRoomIds;
        }

        private void ThrowIfAccessTokenIsEmpty()
        {
            if (state.AccessToken == null)
                throw new InvalidOperationException("No access token has been provided.");
        }

        /*
         * Create a transaction ID
         */
        private string CreateTransactionId()
        {
            var timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            var counter = state.TransactionNumber;

            state.TransactionNumber += 1;

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