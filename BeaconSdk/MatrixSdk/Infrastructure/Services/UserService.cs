// ReSharper disable ArgumentsStyleNamedExpression

namespace MatrixSdk.Infrastructure.Services
{
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Dto.Login;
    using Extensions;
    using Sodium;

    public class UserService
    {
        private const string RequestUri = "_matrix/client/r0";

        private readonly IHttpClientFactory httpClientFactory;

        public UserService(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        private HttpClient CreateHttpClient() => httpClientFactory.CreateClient(MatrixApiConstants.Matrix);

        public async Task<LoginResponse> LoginAsync(KeyPair keyPair, CancellationToken cancellationToken)
        {
            var loginDigest = SignatureCryptoService.GenerateLoginDigest();
            var hexSignature = SignatureCryptoService.GenerateHexSignature(loginDigest, keyPair.PrivateKey);
            var hexPublicKey = SignatureCryptoService.ToHexString(keyPair.PublicKey);
            var hexId = SignatureCryptoService.GenerateHexId(keyPair.PublicKey);

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