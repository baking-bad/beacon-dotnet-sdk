namespace MatrixSdk.Services
{
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Dto;
    using Dto.Room.Create;
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

        public async Task<CreateRoomResponse> CreateRoomAsync(string accessToken, string[]? members, CancellationToken cancellationToken)
        {
            var model = new CreateRoomRequest
            (   
                Invite: members,
                Preset: Preset.TrustedPrivateChat,
                IsDirect : true
            );

            var httpClient = httpClientFactory.CreateClient(MatrixConstants.Matrix);
            httpClient.AddBearerToken(accessToken);

            return await httpClient.PostAsJsonAsync<CreateRoomResponse>($"{RequestUri}/createRoom", model, cancellationToken);
        }

        public async Task<JoinedRoomsResponse> GetJoinedRoomsAsync(string accessToken, CancellationToken cancellationToken)
        {
            var httpClient = httpClientFactory.CreateClient(MatrixConstants.Matrix);
            httpClient.AddBearerToken(accessToken);

            return await httpClient.GetAsJsonAsync<JoinedRoomsResponse>($"{RequestUri}/joined_rooms", cancellationToken);
        }
    }
}