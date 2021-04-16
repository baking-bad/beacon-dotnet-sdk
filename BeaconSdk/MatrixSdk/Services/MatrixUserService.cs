// ReSharper disable ArgumentsStyleNamedExpression

namespace MatrixSdk.Services
{
    using System.Net.Http;
    using System.Threading.Tasks;
    using Extensions;
    using Login;

    public sealed class MatrixUserService
    {
        private const string RequestUri = "_matrix/client/r0";

        private readonly CryptoService cryptoService;
        private readonly IHttpClientFactory httpClientFactory;

        public MatrixUserService(IHttpClientFactory httpClientFactory, CryptoService cryptoService)
        {
            this.httpClientFactory = httpClientFactory;
            this.cryptoService = cryptoService;
        }

        public async Task<ResponseLoginDto> LoginAsync(string seed)
        {
            var loginDigest = cryptoService!.GenerateLoginDigest();
            var keyPair = cryptoService.GenerateKeyPairFromSeed(seed);

            var hexSignature = cryptoService.GenerateHexSignature(loginDigest, keyPair.PrivateKey);
            var hexPublicKey = cryptoService.ToHexString(keyPair.PublicKey);
            var hexId = cryptoService.GenerateHexId(keyPair.PublicKey);

            var password = $"ed:{hexSignature}:{hexPublicKey}";
            var deviceId = hexPublicKey;

            var model = new RequestLoginDto
            (
                new Identifier
                (
                    "m.id.user",
                    hexId
                ),
                password,
                deviceId,
                "m.login.password"
            );

            var httpClient = httpClientFactory.CreateClient(Constants.Matrix);

            return await httpClient.PostAsJsonAsync<ResponseLoginDto>($"{RequestUri}/login", model);
        }
    }
}