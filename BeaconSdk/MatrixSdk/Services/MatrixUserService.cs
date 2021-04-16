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

        private readonly MatrixCryptoService matrixCryptoService;
        private readonly IHttpClientFactory httpClientFactory;

        public MatrixUserService(IHttpClientFactory httpClientFactory, MatrixCryptoService matrixCryptoService)
        {
            this.httpClientFactory = httpClientFactory;
            this.matrixCryptoService = matrixCryptoService;
        }

        public async Task<ResponseLoginDto> LoginAsync(string seed)
        {
            var loginDigest = matrixCryptoService!.GenerateLoginDigest();
            var keyPair = matrixCryptoService.GenerateKeyPairFromSeed(seed);

            var hexSignature = matrixCryptoService.GenerateHexSignature(loginDigest, keyPair.PrivateKey);
            var hexPublicKey = matrixCryptoService.ToHexString(keyPair.PublicKey);
            var hexId = matrixCryptoService.GenerateHexId(keyPair.PublicKey);

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