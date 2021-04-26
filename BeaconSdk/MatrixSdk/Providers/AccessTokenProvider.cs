namespace MatrixSdk.Providers
{
    using System;
    using System.Threading.Tasks;
    using Repositories;
    using Services;

    [Obsolete]
    public sealed class AccessTokenProvider
    {
        private readonly MatrixUserService userService;
        private readonly ISeedRepository inMemorySeedRepository;
        
        private string? accessToken;
        
        public AccessTokenProvider(MatrixUserService userService, InMemorySeedRepository inMemorySeedRepository)
        {
            this.userService = userService;
            this.inMemorySeedRepository = inMemorySeedRepository;
        }

        public async Task<string> GetAccessToken() => accessToken ??= await ObtainAccessToken();

        private async Task<string> ObtainAccessToken()
        {
            // var seed = Guid.NewGuid().ToString(); //Todo: generate once and then store seed?
            
            var seed = inMemorySeedRepository.GetSeed().ToString();
            var responseLogin = await userService!.LoginAsync(seed);

            return responseLogin.AccessToken;
        }
    }
}