namespace Matrix.Sdk.Clients
{
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Core.Infrastructure.Dto.Room.Create;
    using Core.Infrastructure.Dto.Room.Join;
    using Core.Infrastructure.Dto.Room.Joined;
    using Core.Infrastructure.Extensions;

    public class RoomClient : BaseApiClient
    {
        public RoomClient(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        {
        }

        protected override string RequestUri => "_matrix/client/r0";

        public async Task<CreateRoomResponse> CreateRoomAsync(string accessToken, string[]? members,
            CancellationToken cancellationToken)
        {
            var model = new CreateRoomRequest
            (
                Invite: members,
                Preset: Preset.TrustedPrivateChat,
                IsDirect: true
            );

            return await CreateHttpClient(accessToken)
                .PostAsJsonAsync<CreateRoomResponse>($"{RequestUri}/createRoom", model, cancellationToken);
        }

        public async Task<JoinRoomResponse> JoinRoomAsync(string accessToken, string roomId,
            CancellationToken cancellationToken) =>
            await CreateHttpClient(accessToken)
                .PostAsJsonAsync<JoinRoomResponse>($"{RequestUri}/rooms/{roomId}/join", null, cancellationToken);

        public async Task<JoinedRoomsResponse> GetJoinedRoomsAsync(string accessToken,
            CancellationToken cancellationToken) =>
            await CreateHttpClient(accessToken)
                .GetAsJsonAsync<JoinedRoomsResponse>($"{RequestUri}/joined_rooms", cancellationToken);

        public async Task LeaveRoomAsync(string accessToken, string roomId, CancellationToken cancellationToken) =>
            await CreateHttpClient(accessToken).PostAsync($"{RequestUri}/rooms/{roomId}/leave", cancellationToken);
    }
}