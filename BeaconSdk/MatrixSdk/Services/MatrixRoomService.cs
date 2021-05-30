namespace MatrixSdk.Services
{
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Dto;
    using Extensions;

    public class MatrixRoomService
    {
        private const string RequestUri = "_matrix/client/r0";
        private readonly IHttpClientFactory httpClientFactory;

        public MatrixRoomService(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        public async Task<MatrixCreateRoomResponse> CreateRoomAsync(string accessToken, string[]? members, CancellationToken cancellationToken)
        {
            var model = new MatrixCreateRoomRequest
            {
                Invite = members,
                Preset = Preset.TrustedPrivateChat,
                IsDirect = true
            };

            var httpClient = httpClientFactory.CreateClient(Constants.Matrix);
            httpClient.AddBearerToken(accessToken);

            return await httpClient.PostAsJsonAsync<MatrixCreateRoomResponse>($"{RequestUri}/createRoom", model, cancellationToken);
        }

        public async Task<MatrixJoinedRoomsResponse> GetJoinedRoomsAsync(string accessToken, CancellationToken cancellationToken)
        {
            var httpClient = httpClientFactory.CreateClient(Constants.Matrix);
            httpClient.AddBearerToken(accessToken);

            return await httpClient.GetAsJsonAsync<MatrixJoinedRoomsResponse>($"{RequestUri}/joined_rooms", cancellationToken);
        }
    }
}