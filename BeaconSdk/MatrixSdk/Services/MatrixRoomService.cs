namespace MatrixSdk.Services
{
    using System.Net.Http;
    using System.Threading.Tasks;
    using Extensions;
    using MatrixApiDtos;

    public sealed class MatrixRoomService
    {
        private const string RequestUri = "_matrix/client/r0";
        private readonly IHttpClientFactory httpClientFactory;

        public MatrixRoomService(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        public async Task<ResponseCreateRoomDto> CreateRoomAsync(string accessToken, string member)
        {
            var model = new RequestCreateRoomDto
            {
                // Invite = new [] {member},
                Preset = Preset.TrustedPrivateChat,
                IsDirect = true
            };

            var httpClient = httpClientFactory.CreateClient(Constants.Matrix);
            httpClient.AddBearerToken(accessToken);

            return await httpClient.PostAsJsonAsync<ResponseCreateRoomDto>($"{RequestUri}/createRoom", model);
        }

        public async Task<ResponseJoinedRoomsDto> GetJoinedRoomsAsync(string accessToken)
        {
            var httpClient = httpClientFactory.CreateClient(Constants.Matrix);
            httpClient.AddBearerToken(accessToken);

            return await httpClient.GetAsJsonAsync<ResponseJoinedRoomsDto>($"{RequestUri}/joined_rooms");
        }
    }
}