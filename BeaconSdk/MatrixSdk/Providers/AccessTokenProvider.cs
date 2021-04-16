namespace MatrixSdk.Providers
{
    using System;
    using System.Threading.Tasks;
    using Services;

    public sealed class AccessTokenProvider
    {
        private readonly CryptoService cryptoService;
        private readonly MatrixUserService userService;
        private string? accessToken;

        public AccessTokenProvider(MatrixUserService userService, CryptoService cryptoService)
        {
            this.userService = userService;
            this.cryptoService = cryptoService;
        }

        public async Task<string> GetAccessToken() => accessToken ??= await ObtainAccessToken();

        private async Task<string> ObtainAccessToken()
        {
            var seed = Guid.NewGuid().ToString(); //Todo: generate once and then store seed?
            var responseLogin = await userService!.LoginAsync(seed);

            return responseLogin.AccessToken;
        }
    }
}