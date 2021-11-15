namespace Matrix.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Core.Domain;
    using Core.Domain.MatrixRoom;
    using Core.Domain.Network;
    using Core.Domain.Services;
    using Core.Infrastructure.Dto.Login;
    using LoginRequest = Core.Domain.Network.LoginRequest;

    /// <summary>
    ///     A Client for interaction with Matrix.
    /// </summary>
    public class MatrixClient : IMatrixClient
    {
        private readonly CancellationTokenSource _cts = new();

        private readonly INetworkService _networkService;
        private readonly IPollingService _pollingService;

        private string? _accessToken;
        private ulong _transactionNumber;

        public MatrixClient(INetworkService networkService,
            IPollingService pollingService)
        {
            _networkService = networkService;
            _pollingService = pollingService;
        }

        public event EventHandler<MatrixRoomEventsEventArgs> OnMatrixRoomEventsReceived;

        public string UserId { get; private set; }

        public Uri? BaseAddress { get; private set; }

        public bool LoggedIn { get; private set; }

        public MatrixRoom[] InvitedRooms => _pollingService.InvitedRooms;

        public MatrixRoom[] JoinedRooms => _pollingService.JoinedRooms;

        public MatrixRoom[] LeftRooms => _pollingService.LeftRooms;

        public async Task LoginAsync(LoginRequest request)
        {
            LoginResponse response = await _networkService.LoginAsync(request, _cts.Token);

            BaseAddress = request.BaseAddress;
            UserId = response.UserId;
            _accessToken = response.AccessToken;

            _pollingService.Init(BaseAddress, _accessToken);

            LoggedIn = true;
        }

        public void Start()
        {
            if (!LoggedIn)
                throw new Exception("Call LoginAsync first");

            _pollingService.OnSyncBatchReceived += OnSyncBatchReceived;
            _pollingService.Start();
        }

        public void Stop()
        {
            _pollingService.Stop();
            _pollingService.OnSyncBatchReceived -= OnSyncBatchReceived;
        }

        public async Task<MatrixRoom> CreateTrustedPrivateRoomAsync(string[] invitedUserIds)
        {
            var request = new CreateTrustedPrivateRoomRequest(BaseAddress!, _accessToken!, invitedUserIds);
            MatrixRoom matrixRoom = await _networkService.CreateTrustedPrivateRoomAsync(request, _cts.Token);

            _pollingService.UpdateMatrixRoom(matrixRoom.Id, matrixRoom);

            return matrixRoom;
        }

        public async Task<MatrixRoom> JoinTrustedPrivateRoomAsync(string roomId)
        {
            MatrixRoom? matrixRoom = _pollingService.GetMatrixRoom(roomId);
            if (matrixRoom != null)
                return matrixRoom;

            var request = new JoinTrustedPrivateRoomRequest(BaseAddress!, _accessToken!, roomId);
            matrixRoom = await _networkService.JoinTrustedPrivateRoomAsync(request, _cts.Token);

            _pollingService.UpdateMatrixRoom(matrixRoom.Id, matrixRoom);

            return matrixRoom;
        }

        public async Task<string> SendMessageAsync(string roomId, string message)
        {
            string transactionId = CreateTransactionId();

            var request = new SendMessageRequest(BaseAddress!, _accessToken!, roomId, transactionId, message);
            return await _networkService.SendMessageAsync(request, _cts.Token);
        }

        public async Task<List<string>> GetJoinedRoomsIdsAsync()
        {
            var request = new GetJoinedRoomsIdsRequest(BaseAddress!, _accessToken!);
            return await _networkService.GetJoinedRoomsIdsAsync(request, _cts.Token);
        }

        public async Task LeaveRoomAsync(string roomId)
        {
            var request = new LeaveRoomRequest(BaseAddress!, _accessToken!, roomId);
            await _networkService.LeaveRoomAsync(request, _cts.Token);
        }

        private void OnSyncBatchReceived(object? sender, SyncBatchEventArgs syncBatchEventArgs)
        {
            if (sender is not IPollingService)
                throw new ArgumentException("sender is not polling service");

            SyncBatch batch = syncBatchEventArgs.SyncBatch;

            OnMatrixRoomEventsReceived.Invoke(this, new MatrixRoomEventsEventArgs(batch.MatrixRoomEvents));
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