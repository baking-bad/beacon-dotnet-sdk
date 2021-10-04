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

        private readonly IHttpClientFactory _httpClientFactory;

        public UserService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private HttpClient CreateHttpClient() => _httpClientFactory.CreateClient(MatrixApiConstants.Matrix);

        public async Task<LoginResponse> LoginAsync(KeyPair keyPair, CancellationToken cancellationToken)
        {
            var loginDigest = MatrixCryptographyService.GenerateLoginDigest();
            var hexSignature = MatrixCryptographyService.GenerateHexSignature(loginDigest, keyPair.PrivateKey);
            var hexPublicKey = MatrixCryptographyService.ToHexString(keyPair.PublicKey);
            var hexId = MatrixCryptographyService.GenerateHexId(keyPair.PublicKey);

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