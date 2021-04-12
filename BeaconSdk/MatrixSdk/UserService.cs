namespace MatrixSdk
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Login;

    public class UserService
    {
        private readonly IHttpClientFactory httpClientFactory;

        public UserService(IHttpClientFactory httpClientFactory)
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

        public async Task<ResponseLoginDto> LoginAsync(string userId, string password, string deviceId) =>
            await HttpClient.PostAsync<RequestLoginDto, ResponseLoginDto>("_matrix/client/r0/login",
                new RequestLoginDto
                {
                    identifier = new Identifier
                    {
                        type = "m.id.user",
                        user = userId
                    },
                    device_id = deviceId,
                    password = password,
                    type = "m.login.password"
                });
    }
}