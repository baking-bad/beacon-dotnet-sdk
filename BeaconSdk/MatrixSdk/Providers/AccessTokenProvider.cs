namespace MatrixSdk.Providers
{
    using System;
    using System.Threading.Tasks;
    using Services;

    public sealed class AccessTokenProvider
    {
        private readonly MatrixCryptoService matrixCryptoService;
        private readonly MatrixUserService userService;
        private string? accessToken;

        public AccessTokenProvider(MatrixUserService userService, MatrixCryptoService matrixCryptoService)
        {
            this.userService = userService;
            this.matrixCryptoService = matrixCryptoService;
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