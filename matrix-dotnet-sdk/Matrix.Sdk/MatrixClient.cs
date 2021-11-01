namespace Matrix.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Core;
    using Core.Domain.MatrixRoom;
    using Core.Domain.Room;
    using Core.Domain.Services;
    using Core.Infrastructure.Dto.Login;
    using Microsoft.Extensions.Logging;
    using Sodium;

    /// <summary>
    ///     A Client for interaction with Matrix.
    /// </summary>
    public class MatrixClient : IMatrixClient
    {
        private readonly ICryptographyService _cryptographyService;
        private readonly CancellationTokenSource _cts = new(); // todo: refactor.
        private readonly ILogger<MatrixClient> _logger;

        private readonly INetworkService _networkService;
        private readonly IPollingService _pollingService;

        private string? _accessToken;
        private ulong _transactionNumber;

        public MatrixClient(INetworkService networkService,
            IPollingService pollingService,
            ICryptographyService cryptographyService,
            MatrixEventNotifier<List<BaseRoomEvent>> matrixEventNotifier,
            ILogger<MatrixClient> logger)
        {
            _networkService = networkService;
            _pollingService = pollingService;
            _cryptographyService = cryptographyService;

            MatrixEventNotifier = matrixEventNotifier;
            _logger = logger;
        }

        public string UserId { get; private set; }
        public Uri? BaseAddress { get; private set; }

        public MatrixEventNotifier<List<BaseRoomEvent>> MatrixEventNotifier { get; }

        public MatrixRoom[] InvitedRooms => _pollingService.InvitedRooms;

        public MatrixRoom[] JoinedRooms => _pollingService.JoinedRooms;

        public MatrixRoom[] LeftRooms => _pollingService.LeftRooms;

        public async Task StartAsync(Uri? baseAddress, KeyPair keyPair)
        {
            _logger.LogInformation("Matrix client: Starting");
            BaseAddress = baseAddress ?? new Uri(Constants.FallBackNodeAddress);

            byte[] loginDigest = _cryptographyService.GenerateLoginDigest();
            string hexSignature = _cryptographyService.GenerateHexSignature(loginDigest, keyPair.PrivateKey);
            string publicKeyHex = _cryptographyService.ToHexString(keyPair.PublicKey);
            string hexId = _cryptographyService.GenerateHexId(keyPair.PublicKey);

            var password = $"ed:{hexSignature}:{publicKeyHex}";
            string deviceId = publicKeyHex;

            LoginResponse response =
                await _networkService.LoginAsync(BaseAddress, hexId, password, deviceId, _cts.Token);

            UserId = response.UserId;
            _accessToken = response.AccessToken;

            _pollingService.Start(BaseAddress, _accessToken,
                batch => { MatrixEventNotifier.NotifyAll(batch.MatrixRoomEvents); });

            _logger.LogInformation("Matrix client: Logged in and began sync");
        }

        public async Task StopAsync()
        {
            _logger.LogInformation("Matrix client: Stopping");

            await Task.CompletedTask;

            _pollingService.Stop();

            _logger.LogInformation("Matrix client: Stopped");
        }

        public async Task<MatrixRoom> CreateTrustedPrivateRoomAsync(string[] invitedUserIds)
        {
            MatrixRoom matrixRoom =
                await _networkService.CreateTrustedPrivateRoomAsync(BaseAddress!, _accessToken!, invitedUserIds,
                    _cts.Token);

            _pollingService.UpdateMatrixRoom(matrixRoom.Id, matrixRoom);

            return matrixRoom;
        }

        public async Task<MatrixRoom> JoinTrustedPrivateRoomAsync(string roomId)
        {
            MatrixRoom? matrixRoom = _pollingService.GetMatrixRoom(roomId);
            if (matrixRoom != null)
                return matrixRoom;

            matrixRoom =
                await _networkService.JoinTrustedPrivateRoomAsync(BaseAddress!, _accessToken!, roomId, _cts.Token);

            _pollingService.UpdateMatrixRoom(matrixRoom.Id, matrixRoom);

            return matrixRoom;
        }

        public async Task<string> SendMessageAsync(string roomId, string message)
        {
            string transactionId = CreateTransactionId();

            return await _networkService.SendMessageAsync(BaseAddress!, _accessToken!, roomId, transactionId, message,
                _cts.Token);
        }

        public async Task<List<string>> GetJoinedRoomsIdsAsync() =>
            await _networkService.GetJoinedRoomsIdsAsync(BaseAddress!, _accessToken!, _cts.Token);

        public async Task LeaveRoomAsync(string roomId) =>
            await _networkService.LeaveRoomAsync(BaseAddress!, _accessToken!, roomId, _cts.Token);

        private string CreateTransactionId()
        {
            long timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            ulong counter = _transactionNumber;

            _transactionNumber += 1;

            return $"m{timestamp}.{counter}";
        }
    }
}