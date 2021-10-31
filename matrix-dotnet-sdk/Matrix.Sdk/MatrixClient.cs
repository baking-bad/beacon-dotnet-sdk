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
    using Sodium;
    using LoginRequest = Core.Domain.Network.LoginRequest;

    /// <summary>
    /// A Client for interaction with Matrix.
    /// </summary>
    public class MatrixClient : IMatrixClient
    {
        private readonly ICryptographyService _cryptographyService;
        private readonly CancellationTokenSource _cts = new();
        private readonly INetworkService _networkService;
        private readonly ISyncService _syncService;

        private string? _accessToken;
        private Uri? _baseAddress;
        private ulong _transactionNumber;

        public MatrixClient(INetworkService networkService,
            ISyncService syncService,
            ICryptographyService cryptographyService,
            MatrixEventNotifier<List<BaseRoomEvent>> matrixEventNotifier)
        {
            _networkService = networkService;
            _syncService = syncService;
            _cryptographyService = cryptographyService;

            MatrixEventNotifier = matrixEventNotifier;
        }

        public string UserId { get; private set; }

        public MatrixEventNotifier<List<BaseRoomEvent>> MatrixEventNotifier { get; }

        public MatrixRoom[] InvitedRooms => _syncService.InvitedRooms;

        public MatrixRoom[] JoinedRooms => _syncService.JoinedRooms;

        public MatrixRoom[] LeftRooms => _syncService.LeftRooms;

        public async Task LoginAsync(Uri? baseAddress, KeyPair keyPair)
        {
            _baseAddress = baseAddress ?? new Uri(Constants.FallBackNodeAddress);

            byte[] loginDigest = _cryptographyService.GenerateLoginDigest();
            string hexSignature = _cryptographyService.GenerateHexSignature(loginDigest, keyPair.PrivateKey);
            string publicKeyHex = _cryptographyService.ToHexString(keyPair.PublicKey);
            string hexId = _cryptographyService.GenerateHexId(keyPair.PublicKey);

            var password = $"ed:{hexSignature}:{publicKeyHex}";
            string deviceId = publicKeyHex;

            var request = new LoginRequest(_baseAddress, hexId, password, deviceId);
            LoginResponse response = await _networkService.LoginAsync(request, _cts.Token);

            UserId = response.UserId;
            _accessToken = response.AccessToken;
        }

        public void Start()
        {
            if (_accessToken == null)
                throw new NullReferenceException("You need to login first.");

            _syncService.Start(_baseAddress!, _accessToken!, _cts.Token,
                batch => { MatrixEventNotifier.NotifyAll(batch.MatrixRoomEvents); });
        }

        public void Stop()
        {
            _cts.Cancel();
            _syncService.Stop();
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