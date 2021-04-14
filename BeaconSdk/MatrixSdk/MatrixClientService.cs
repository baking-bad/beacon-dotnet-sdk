// ReSharper disable ArgumentsStyleNamedExpression
namespace MatrixSdk
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using Login;
    using MatrixApiDtos;

    public class MatrixClientService
    {
        private const string requestUri = "_matrix/client/r0";
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

            return await HttpClient.PostAsJsonAsync<ResponseLoginDto>($"{requestUri}/login", model);
        }

        public async Task<ResponseCreateRoomDto> CreateRoomAsync(string accessToken, string member)
        {
            var model = new RequestCreateRoomDto(
                Visibility: Visibility.Private, 
                Invite: new []{member},
                Preset: Preset.TrustedPrivateChat,
                IsDirect: true);

            var httpClient = HttpClient;
            // httpClient.DefaultRequestHeaders.Authorization =
            //     new AuthenticationHeaderValue("Bearer", accessToken);
            
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
            
            return await httpClient.PostAsJsonAsync<ResponseCreateRoomDto>($"{requestUri}/createRoom", model);
        }
    }
}