namespace MatrixSdk
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Dto.Room.Event;
    using Dto.Room.Event.State.Content;
    using Extensions;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using Services;

    public class MatrixClient
    {
        private readonly CancellationTokenSource cancellationTokenSource = new();
        private readonly ILogger<MatrixClient> logger;

        private readonly MatrixEventService matrixEventService;
        private readonly MatrixRoomService matrixRoomService;
        private readonly MatrixUserService matrixUserService;

        private Timer pollingTimer;
        private MatrixClientState state;
        
        public string UserId => state.UserId!;

        public MatrixClient(ILogger<MatrixClient> logger, MatrixUserService matrixUserService, MatrixRoomService matrixRoomService,
            MatrixEventService matrixEventService)
        {
            this.logger = logger;
            this.matrixUserService = matrixUserService;
            this.matrixRoomService = matrixRoomService;
            this.matrixEventService = matrixEventService;
        }

        public async Task StartAsync(string seed)
        {
            logger.LogInformation("Start matrix client ...");

            var response = await matrixUserService!.LoginAsync(seed, cancellationTokenSource.Token);

            state = new MatrixClientState
            {
                UserId = response.UserId,
                AccessToken = response.AccessToken,
                Timeout = 0,
                TransactionNumber = 0
            };

            logger.LogInformation("Start polling ...");
            pollingTimer = new Timer(async _ => await PollAsync(cancellationTokenSource.Token));
            pollingTimer.Change(TimeSpan.Zero, TimeSpan.FromMilliseconds(-1));
        }

        private async Task PollAsync(CancellationToken cancellationToken)
        {
            pollingTimer.Change(Timeout.Infinite, Timeout.Infinite);

            ThrowIfAccessTokenIsEmpty();

            var response = await matrixEventService.SyncAsync(state.AccessToken!, timeout: state.Timeout, nextBatch: state.NextBatch!,
                cancellationToken: cancellationToken);
            
            state.Timeout = 30000;
            state.NextBatch = response.NextBatch;

            logger.LogInformation($"Id: {UserId.TruncateLongString(5)}, Invite: {response.Rooms.Invite.Count}");
            logger.LogInformation($"Id: {UserId.TruncateLongString(5)}, Join: {response.Rooms.Join.Count}");
            logger.LogInformation($"Id: {UserId.TruncateLongString(5)}, Leave: {response.Rooms.Leave.Count}");

            if (response.Rooms.Invite.Count > 0)
            {
                var joined = response.Rooms.Invite;

                foreach (var (roomId,joinedRoom) in joined)
                {
                    var joinedRoomEvents = joinedRoom.InviteState.Events;
                    foreach (var joinedRoomEvent in joinedRoomEvents)
                    {
                        var type = joinedRoomEvent.Type;
                        switch (type)
                        {
                            case RoomEventType.Create:
                                var roomCreateContent = joinedRoomEvent.Content.ToObject<RoomCreateContent>(); 
                                break;
                            case RoomEventType.JoinRules:
                                var roomJoinRulesContent = joinedRoomEvent.Content.ToObject<RoomJoinRulesContent>(); 
                                break;
                            case RoomEventType.Member:
                                var roomMemberContent = joinedRoomEvent.Content.ToObject<RoomMemberContent>(); 
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                }
            }
            
            
            pollingTimer.Change(TimeSpan.Zero, TimeSpan.FromMilliseconds(-1));
        }
        
        
        public async Task CreateTrustedPrivateRoomAsync(string[]? members = null)
        {
            ThrowIfAccessTokenIsEmpty();

            await matrixRoomService.CreateRoomAsync(state.AccessToken!, members, cancellationTokenSource.Token);
        }

        public async Task<List<string>> GetJoinedRoomsAsync()
        {
            ThrowIfAccessTokenIsEmpty();

            var response = await matrixRoomService.GetJoinedRoomsAsync(state.AccessToken!, cancellationTokenSource.Token);

            return response.JoinedRooms;
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
            var timestamp = DateTimeOffset.Now;
            var counter = state.TransactionNumber;

            state.TransactionNumber += 1;

            return $"m{timestamp}.{counter}";
        }
    }
}