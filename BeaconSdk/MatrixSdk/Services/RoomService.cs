namespace MatrixSdk.Services
{
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;
    using Dto.Room.Create;
    using Dto.Room.Join;
    using Dto.Room.Joined;
    using Extensions;

    public class RoomService
    {
        private const string RequestUri = "_matrix/client/r0";
        private readonly IHttpClientFactory httpClientFactory;

        public RoomService(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        private HttpClient CreateHttpClient(string accessToken)
        {
            var httpClient = httpClientFactory.CreateClient(MatrixConstants.Matrix);
            httpClient.AddBearerToken(accessToken);

            return httpClient;
        }

        public async Task<CreateRoomResponse> CreateRoomAsync(string accessToken, string[]? members, CancellationToken cancellationToken)
        {
            var model = new CreateRoomRequest
            (
                Invite: members,
                Preset: Preset.TrustedPrivateChat,
                IsDirect: true
            );

            return await CreateHttpClient(accessToken).PostAsJsonAsync<CreateRoomResponse>($"{RequestUri}/createRoom", model, cancellationToken);
        }

        public async Task<JoinRoomResponse> JoinRoomAsync(string accessToken, string roomId, CancellationToken cancellationToken) =>
            await CreateHttpClient(accessToken)
                .PostAsJsonAsync<JoinRoomResponse>($"{RequestUri}/rooms/{roomId}/join", null, cancellationToken);
        
        public async Task<JoinedRoomsResponse> GetJoinedRoomsAsync(string accessToken, CancellationToken cancellationToken) =>
            await CreateHttpClient(accessToken)
                .GetAsJsonAsync<JoinedRoomsResponse>($"{RequestUri}/joined_rooms", cancellationToken);
    }
}