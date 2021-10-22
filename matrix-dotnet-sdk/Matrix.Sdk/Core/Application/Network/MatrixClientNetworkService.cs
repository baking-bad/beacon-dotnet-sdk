namespace Matrix.Sdk.Core.Application.Network
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Domain;
    using Infrastructure.Dto.Event;
    using Infrastructure.Dto.Login;
    using Infrastructure.Dto.Room.Create;
    using Infrastructure.Dto.Room.Join;
    using Infrastructure.Dto.Room.Joined;
    using Infrastructure.Dto.Sync;
    using Infrastructure.Services;
    using Sodium;

    public class MatrixClientNetworkService : INetworkService
    {
        private readonly EventService _eventService;
        private readonly RoomService _roomService;

        private readonly UserService _userService;

        public MatrixClientNetworkService(UserService userService, EventService eventService, RoomService roomService)
        {
            _userService = userService;
            _eventService = eventService;
            _roomService = roomService;
        }

        public async Task<LoginResponse> LoginAsync(CancellationToken cancellationToken, KeyPair keyPair) =>
            await _userService.LoginAsync(keyPair, cancellationToken);

        public async Task<SyncResponse> SyncAsync(string accessToken, CancellationToken cancellationToken,
            ulong? timeout = null,
            string? nextBatch = null)
        {
            ThrowIfAccessTokenIsEmpty(accessToken);

            return await _eventService.SyncAsync(accessToken, cancellationToken, timeout, nextBatch);
        }

        public async Task<MatrixRoom> CreateTrustedPrivateRoomAsync(ClientStateManager stateManager,
            CancellationToken cancellationToken,
            string[]? invitedUserIds = null)
        {
            ThrowIfAccessTokenIsEmpty(stateManager.AccessToken);

            CreateRoomResponse response =
                await _roomService.CreateRoomAsync(stateManager.AccessToken!, invitedUserIds, cancellationToken);
            var matrixRoom = new MatrixRoom(response.RoomId, MatrixRoomStatus.Unknown);
            stateManager.UpdateMatrixRoom(response.RoomId, matrixRoom);

            return matrixRoom;
        }

        public async Task<MatrixRoom> JoinTrustedPrivateRoomAsync(ClientStateManager stateManager,
            CancellationToken cancellationToken, string roomId)
        {
            if (stateManager.MatrixRooms.TryGetValue(roomId, out MatrixRoom matrixRoom))
                return matrixRoom;

            ThrowIfAccessTokenIsEmpty(stateManager.AccessToken);

            JoinRoomResponse response =
                await _roomService.JoinRoomAsync(stateManager.AccessToken!, roomId, cancellationToken);
            matrixRoom = new MatrixRoom(response.RoomId, MatrixRoomStatus.Unknown);
            stateManager.UpdateMatrixRoom(response.RoomId, matrixRoom);

            return matrixRoom;
        }

        public async Task<string?> SendMessageAsync(ClientStateManager stateManager,
            CancellationToken cancellationToken, string roomId, string message)
        {
            ThrowIfAccessTokenIsEmpty(stateManager.AccessToken);

            string transactionId = CreateTransactionId(stateManager);
            EventResponse result = await _eventService.SendMessageAsync(stateManager.AccessToken!, cancellationToken,
                roomId, transactionId, message);
            // var id = result.EventId;

            return result.EventId;
        }

        public async Task<List<string>> GetJoinedRoomsIdsAsync(string accessToken, CancellationToken cancellationToken)
        {
            ThrowIfAccessTokenIsEmpty(accessToken);

            JoinedRoomsResponse response = await _roomService.GetJoinedRoomsAsync(accessToken!, cancellationToken);

            return response.JoinedRoomIds;
        }

        public async Task LeaveRoomAsync(string accessToken, CancellationToken cancellationToken, string roomId)
        {
            ThrowIfAccessTokenIsEmpty(accessToken);

            await _roomService.LeaveRoomAsync(accessToken!, roomId, cancellationToken);
        }

        private string CreateTransactionId(ClientStateManager clientStateManager)
        {
            long timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            ulong counter = clientStateManager.TransactionNumber;

            clientStateManager.TransactionNumber += 1;

            return $"m{timestamp}.{counter}";
        }

        private void ThrowIfAccessTokenIsEmpty(string accessToken)
        {
            if (accessToken == null)
                throw new InvalidOperationException("No access token has been provided.");
        }
    }
}