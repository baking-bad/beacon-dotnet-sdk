namespace MatrixSdk.Infrastructure.Providers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Repositories;
    using Services;

    [Obsolete]
    internal sealed class AccessTokenProvider
    {
        private readonly ISeedRepository memorySeedRepository;
        private readonly UserService userService;

        private string? accessToken;

        public AccessTokenProvider(UserService userService, MemorySeedRepository memorySeedRepository)
        {
            this.userService = userService;
            this.memorySeedRepository = memorySeedRepository;
        }

        public async Task<string> GetAccessToken() => accessToken ??= await ObtainAccessToken();

        private async Task<string> ObtainAccessToken()
        {
            // var seed = Guid.NewGuid().ToString(); //Todo: generate once and then store seed?

            var seed = memorySeedRepository.GetSeed().ToString();
            var responseLogin = await userService!.LoginAsync(seed, CancellationToken.None);

            return responseLogin.AccessToken;
        }
    }
}