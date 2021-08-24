namespace MatrixSdk.Application.Network
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Domain;
    using Infrastructure.Dto.Login;
    using Infrastructure.Dto.Sync;
    using Infrastructure.Services;
    using Sodium;

    public class MatrixClientNetworkService : INetworkService
    {
        private readonly EventService eventService;
        private readonly RoomService roomService;

        private readonly UserService userService;

        public MatrixClientNetworkService(UserService userService, EventService eventService, RoomService roomService)
        {
            this.userService = userService;
            this.eventService = eventService;
            this.roomService = roomService;
        }

        public async Task<LoginResponse> LoginAsync(CancellationToken cancellationToken, KeyPair keyPair) =>
            await userService.LoginAsync(keyPair, cancellationToken);

        public async Task<SyncResponse> SyncAsync(string accessToken, CancellationToken cancellationToken, ulong? timeout = null,
            string? nextBatch = null)
        {
            ThrowIfAccessTokenIsEmpty(accessToken);

            return await eventService.SyncAsync(accessToken, cancellationToken, timeout, nextBatch);
        }

        public async Task<MatrixRoom> CreateTrustedPrivateRoomAsync(ClientStateManager stateManager, CancellationToken cancellationToken,
            string[]? invitedUserIds = null)
        {
            ThrowIfAccessTokenIsEmpty(stateManager.AccessToken);

            var response = await roomService.CreateRoomAsync(stateManager.AccessToken!, invitedUserIds, cancellationToken);
            var matrixRoom = new MatrixRoom(response.RoomId, MatrixRoomStatus.Unknown);
            stateManager.UpdateMatrixRoom(response.RoomId, matrixRoom);

            return matrixRoom;
        }

        public async Task<MatrixRoom> JoinTrustedPrivateRoomAsync(ClientStateManager stateManager, CancellationToken cancellationToken, string roomId)
        {
            if (stateManager.MatrixRooms.TryGetValue(roomId, out var matrixRoom))
                return matrixRoom;

            ThrowIfAccessTokenIsEmpty(stateManager.AccessToken);

            var response = await roomService.JoinRoomAsync(stateManager.AccessToken!, roomId, cancellationToken);
            matrixRoom = new MatrixRoom(response.RoomId, MatrixRoomStatus.Unknown);
            stateManager.UpdateMatrixRoom(response.RoomId, matrixRoom);

            return matrixRoom;
        }

        public async Task SendMessageAsync(ClientStateManager stateManager, CancellationToken cancellationToken, string roomId, string message)
        {
            ThrowIfAccessTokenIsEmpty(stateManager.AccessToken);

            var transactionId = CreateTransactionId(stateManager);
            var result = await eventService.SendMessageAsync(stateManager.AccessToken!, cancellationToken, roomId, transactionId, message);
            var id = result.EventId;
        }

        public async Task<List<string>> GetJoinedRoomsIdsAsync(string accessToken, CancellationToken cancellationToken)
        {
            ThrowIfAccessTokenIsEmpty(accessToken);

            var response = await roomService.GetJoinedRoomsAsync(accessToken!, cancellationToken);

            return response.JoinedRoomIds;
        }

        public async Task LeaveRoomAsync(string accessToken, CancellationToken cancellationToken, string roomId)
        {
            ThrowIfAccessTokenIsEmpty(accessToken);

            await roomService.LeaveRoomAsync(accessToken!, roomId, cancellationToken);
        }

        private string CreateTransactionId(ClientStateManager clientStateManager)
        {
            var timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            var counter = clientStateManager.TransactionNumber;

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