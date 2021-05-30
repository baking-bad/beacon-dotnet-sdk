// ReSharper disable ArgumentsStyleNamedExpression

namespace MatrixSdk.Services
{
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Dto;
    using Extensions;

    public class MatrixUserService
    {
        private const string RequestUri = "_matrix/client/r0";
        private readonly IHttpClientFactory httpClientFactory;

        private readonly MatrixCryptoService matrixCryptoService;

        public MatrixUserService(IHttpClientFactory httpClientFactory, MatrixCryptoService matrixCryptoService)
        {
            this.httpClientFactory = httpClientFactory;
            this.matrixCryptoService = matrixCryptoService;
        }

        public async Task<MatrixLoginResponse> LoginAsync(string seed, CancellationToken cancellationToken)
        {
            var loginDigest = matrixCryptoService!.GenerateLoginDigest();
            var keyPair = matrixCryptoService.GenerateKeyPairFromSeed(seed);

            var hexSignature = matrixCryptoService.GenerateHexSignature(loginDigest, keyPair.PrivateKey);
            var hexPublicKey = matrixCryptoService.ToHexString(keyPair.PublicKey);
            var hexId = matrixCryptoService.GenerateHexId(keyPair.PublicKey);

            var password = $"ed:{hexSignature}:{hexPublicKey}";
            var deviceId = hexPublicKey;

            var model = new MatrixLoginRequest
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

            return await httpClient.PostAsJsonAsync<MatrixLoginResponse>($"{RequestUri}/login", model, cancellationToken);
        }
    }
}