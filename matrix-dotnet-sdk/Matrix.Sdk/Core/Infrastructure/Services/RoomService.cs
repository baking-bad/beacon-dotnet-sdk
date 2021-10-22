namespace Matrix.Sdk.Core.Infrastructure.Services
{
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Dto.Room.Create;
    using Dto.Room.Join;
    using Dto.Room.Joined;
    using Extensions;

    public class RoomService
    {
        private const string RequestUri = "_matrix/client/r0";
        private readonly IHttpClientFactory _httpClientFactory;

        public RoomService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private HttpClient CreateHttpClient(string accessToken)
        {
            HttpClient httpClient = _httpClientFactory.CreateClient(MatrixApiConstants.Matrix);
            httpClient.AddBearerToken(accessToken);

            return httpClient;
        }

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