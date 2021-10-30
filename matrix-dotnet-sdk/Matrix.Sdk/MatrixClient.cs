namespace Matrix.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Core;
    using Core.Domain.MatrixRoom;
    using Core.Domain.Network;
    using Core.Domain.Room;
    using Core.Domain.Services;
    using Core.Infrastructure.Dto.Login;
    using Microsoft.Extensions.Logging;
    using Sodium;
    using LoginRequest = Core.Domain.Services.LoginRequest;

    public class MatrixClient : IMatrixClient
    {
        private readonly CancellationTokenSource _cts = new();
        private readonly ILogger<MatrixClient> _logger;
        private readonly INetworkService _networkService;
        private readonly ISyncService _syncService;

        private string? _accessToken;
        private Uri? _baseAddress;
        private ulong _transactionNumber;

        public MatrixClient(ILogger<MatrixClient> logger, INetworkService networkService, ISyncService syncService,
            MatrixEventNotifier<List<BaseRoomEvent>> matrixEventNotifier)
        {
            _logger = logger;
            _networkService = networkService;
            _syncService = syncService;
            MatrixEventNotifier = matrixEventNotifier;
        }

        public string UserId { get; private set; }
        
        public MatrixEventNotifier<List<BaseRoomEvent>> MatrixEventNotifier { get; }

        public MatrixRoom[] InvitedRooms => _syncService.InvitedRooms;

        public MatrixRoom[] JoinedRooms => _syncService.JoinedRooms;

        public MatrixRoom[] LeftRooms => _syncService.LeftRooms;

        public async Task StartAsync(Uri? baseAddress, KeyPair keyPair)
        {
            _logger.LogInformation("MatrixClient: Starting...");

            _baseAddress = baseAddress ?? new Uri(Constants.FallBackAddress);

            var request = new LoginRequest(_baseAddress, keyPair);
            LoginResponse response = await _networkService.LoginAsync(request, _cts.Token);

            UserId = response.UserId;
            _accessToken = response.AccessToken;

            _syncService.Start(_baseAddress, _accessToken, _cts.Token,
                batch => { MatrixEventNotifier.NotifyAll(batch.MatrixRoomEvents); });

            _logger.LogInformation("MatrixClient: Ready");
        }

        public async Task StopAsync()
        {
            _logger.LogInformation("MatrixClient: Stopping...");

            await Task.Yield();

            _cts.Cancel();
            _syncService.Stop();

            _logger.LogInformation("MatrixClient: Stopped");
        }

        public async Task<MatrixRoom> CreateTrustedPrivateRoomAsync(string[] invitedUserIds)
        {
            var request =
                new CreateTrustedPrivateRoomRequest(_baseAddress!, _accessToken!, invitedUserIds);

            MatrixRoom matrixRoom = await _networkService.CreateTrustedPrivateRoomAsync(request, _cts.Token);

            _syncService.UpdateMatrixRoom(matrixRoom.Id, matrixRoom);

            return matrixRoom;
        }

        public async Task<MatrixRoom> JoinTrustedPrivateRoomAsync(string roomId)
        {
            MatrixRoom? matrixRoom = _syncService.GetMatrixRoom(roomId);
            if (matrixRoom != null)
                return matrixRoom;

            var request = new JoinTrustedPrivateRoomRequest(_baseAddress!, _accessToken!, roomId);
            matrixRoom = await _networkService.JoinTrustedPrivateRoomAsync(request, _cts.Token);

            _syncService.UpdateMatrixRoom(matrixRoom.Id, matrixRoom);

            return matrixRoom;
        }

        public async Task<string> SendMessageAsync(string roomId, string message)
        {
            string transactionId = CreateTransactionId();

            var request = new SendMessageRequest(_baseAddress!, _accessToken!, roomId, transactionId, message);
            return await _networkService.SendMessageAsync(request, _cts.Token);
        }

        public async Task<List<string>> GetJoinedRoomsIdsAsync()
        {
            var request = new GetJoinedRoomsIdsRequest(_baseAddress!, _accessToken!);
            return await _networkService.GetJoinedRoomsIdsAsync(request, _cts.Token);
        }

        public async Task LeaveRoomAsync(string roomId)
        {
            var request = new LeaveRoomRequest(_baseAddress!, _accessToken!, roomId);
            await _networkService.LeaveRoomAsync(request, _cts.Token);
        }

        private string CreateTransactionId()
        {
            long timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            ulong counter = _transactionNumber;

            _transactionNumber += 1;

            return $"m{timestamp}.{counter}";
        }
    }
}