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

        public async Task<LoginResponse> LoginAsync(Uri baseAddress, string user, string password, string deviceId,
            CancellationToken cancellationToken) =>
            await _userService.LoginAsync(baseAddress, user, password, deviceId, cancellationToken);

        public async Task<MatrixRoom> CreateTrustedPrivateRoomAsync(Uri baseAddress, string accessToken,
            string[] invitedUserIds,
            CancellationToken cancellationToken)
        {
            CreateRoomResponse response =
                await _roomService.CreateRoomAsync(baseAddress, accessToken, invitedUserIds,
                    cancellationToken);

            return new MatrixRoom(response.RoomId, MatrixRoomStatus.Unknown);
        }

        public async Task<MatrixRoom> JoinTrustedPrivateRoomAsync(Uri baseAddress, string accessToken, string roomId,
            CancellationToken cancellationToken)
        {
            JoinRoomResponse response =
                await _roomService.JoinRoomAsync(baseAddress, accessToken, roomId,
                    cancellationToken);

            return new MatrixRoom(response.RoomId, MatrixRoomStatus.Unknown);
        }

        public async Task<string> SendMessageAsync(Uri baseAddress, string accessToken, string roomId,
            string transactionId, string message, CancellationToken cancellationToken)
        {
            EventResponse eventResponse = await _eventService.SendMessageAsync(baseAddress, accessToken,
                cancellationToken, roomId, transactionId, message);

            if (eventResponse.EventId == null)
                throw new NullReferenceException(nameof(eventResponse.EventId));

            return eventResponse.EventId;
        }

        public async Task<List<string>> GetJoinedRoomsIdsAsync(Uri baseAddress, string accessToken,
            CancellationToken cancellationToken)
        {
            JoinedRoomsResponse response =
                await _roomService.GetJoinedRoomsAsync(baseAddress, accessToken, cancellationToken);

            return response.JoinedRoomIds;
        }

        public async Task LeaveRoomAsync(Uri baseAddress, string accessToken, string roomId,
            CancellationToken cancellationToken) =>
            await _roomService.LeaveRoomAsync(baseAddress, accessToken, roomId, cancellationToken);
    }
}