// ReSharper disable ArgumentsStyleNamedExpression

namespace MatrixSdk.Infrastructure.Services
{
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Dto.Login;
    using Extensions;

    public class UserService
    {
        private const string RequestUri = "_matrix/client/r0";

        private readonly CryptoService cryptoService;
        private readonly IHttpClientFactory httpClientFactory;


        public UserService(IHttpClientFactory httpClientFactory, CryptoService cryptoService)
        {
            this.httpClientFactory = httpClientFactory;
            this.cryptoService = cryptoService;
        }
        private HttpClient CreateHttpClient() => httpClientFactory.CreateClient(MatrixApiConstants.Matrix);

        public async Task<LoginResponse> LoginAsync(string seed, CancellationToken cancellationToken)
        {
            var loginDigest = cryptoService!.GenerateLoginDigest();
            var keyPair = cryptoService.GenerateKeyPairFromSeed(seed);

            var hexSignature = cryptoService.GenerateHexSignature(loginDigest, keyPair.PrivateKey);
            var hexPublicKey = cryptoService.ToHexString(keyPair.PublicKey);
            var hexId = cryptoService.GenerateHexId(keyPair.PublicKey);

            var password = $"ed:{hexSignature}:{hexPublicKey}";
            var deviceId = hexPublicKey;

            var model = new LoginRequest
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

            return await CreateHttpClient().PostAsJsonAsync<LoginResponse>($"{RequestUri}/login", model, cancellationToken);
        }
    }
}