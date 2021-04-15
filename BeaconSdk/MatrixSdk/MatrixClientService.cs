// ReSharper disable ArgumentsStyleNamedExpression

namespace MatrixSdk
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Extensions;
    using Login;
    using MatrixApiDtos;

    public class MatrixClientService
    {
        private const string RequestUri = "_matrix/client/r0";
        private readonly IHttpClientFactory httpClientFactory;

        public MatrixClientService(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        private HttpClient HttpClient
        {
            get
            {
                var client = httpClientFactory.CreateClient();
                client.BaseAddress = new Uri("https://matrix.papers.tech/");
                return client;
            }
        }

        public async Task<ResponseLoginDto> LoginAsync(string userId, string password, string deviceId)
        {
            var model = new RequestLoginDto
            (
                new Identifier
                (
                    "m.id.user",
                    userId
                ),
                password,
                deviceId,
                "m.login.password"
            );

            return await HttpClient.PostAsJsonAsync<ResponseLoginDto>($"{RequestUri}/login", model);
        }

        public async Task<ResponseCreateRoomDto> CreateRoomAsync(string accessToken, string member)
        {
            var model = new RequestCreateRoomDto
            {
                // Invite = new [] {member},
                Preset = Preset.TrustedPrivateChat,
                IsDirect = true
            };

            var httpClient = HttpClient;
            httpClient.AddBearerToken(accessToken);

            return await httpClient.PostAsJsonAsync<ResponseCreateRoomDto>($"{RequestUri}/createRoom", model);
        }

        public record RequestCreateRoomDto2(string room_alias_name);

        // return await httpClient.PostAsJsonAsync<ResponseCreateRoomDto>($"{RequestUri}/createRoom", 
        //     new RequestCreateRoomDto2("dfgsdgsgsfsdsdfsrewrwrdgsdg"));
    }
}