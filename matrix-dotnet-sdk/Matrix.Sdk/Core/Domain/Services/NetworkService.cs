namespace Matrix.Sdk.Core.Domain.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Infrastructure.Dto.Event;
    using Infrastructure.Dto.Login;
    using Infrastructure.Dto.Room.Create;
    using Infrastructure.Dto.Room.Join;
    using Infrastructure.Dto.Room.Joined;
    using Infrastructure.Services;
    using MatrixRoom;
    using Network;
    using LoginRequest = Network.LoginRequest;

    public class NetworkService : INetworkService
    {
        private readonly EventService _eventService;
        private readonly RoomService _roomService;
        private readonly UserService _userService;

        public NetworkService(EventService eventService, RoomService roomService, UserService userService)
        {
            _eventService = eventService;
            _roomService = roomService;
            _userService = userService;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken) =>
            await _userService.LoginAsync(request.BaseAddress, request.User, request.Password, request.DeviceId,
                cancellationToken);

        public async Task<MatrixRoom> CreateTrustedPrivateRoomAsync(CreateTrustedPrivateRoomRequest request,
            CancellationToken cancellationToken)
        {
            CreateRoomResponse response =
                await _roomService.CreateRoomAsync(request.BaseAddress, request.AccessToken, request.InvitedUserIds,
                    cancellationToken);

            return new MatrixRoom(response.RoomId, MatrixRoomStatus.Unknown);
        }

        public async Task<MatrixRoom> JoinTrustedPrivateRoomAsync(JoinTrustedPrivateRoomRequest request,
            CancellationToken cancellationToken)
        {
            JoinRoomResponse response =
                await _roomService.JoinRoomAsync(request.BaseAddress, request.AccessToken, request.RoomId,
                    cancellationToken);

            return new MatrixRoom(response.RoomId, MatrixRoomStatus.Unknown);
        }

        public async Task<string> SendMessageAsync(SendMessageRequest request, CancellationToken cancellationToken)
        {
            EventResponse eventResponse = await _eventService.SendMessageAsync(request.BaseAddress, request.AccessToken,
                cancellationToken,
                request.RoomId, request.TransactionId, request.Message);

            if (eventResponse.EventId == null)
                throw new NullReferenceException(nameof(eventResponse.EventId));

            return eventResponse.EventId;
        }

        public async Task<List<string>> GetJoinedRoomsIdsAsync(GetJoinedRoomsIdsRequest request,
            CancellationToken cancellationToken)
        {
            JoinedRoomsResponse response =
                await _roomService.GetJoinedRoomsAsync(request.BaseAddress, request.AccessToken, cancellationToken);

            return response.JoinedRoomIds;
        }

        public async Task LeaveRoomAsync(LeaveRoomRequest request, CancellationToken cancellationToken) =>
            await _roomService.LeaveRoomAsync(request.BaseAddress, request.AccessToken, request.RoomId,
                cancellationToken);
    }
}