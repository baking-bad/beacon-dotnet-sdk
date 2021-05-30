namespace MatrixSdk
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
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

            Console.WriteLine($"Invite: {response.Rooms.Invite.Count}");
            Console.WriteLine($"Join: {response.Rooms.Join.Count}");
            Console.WriteLine($"Leave: {response.Rooms.Leave.Count}");
            
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