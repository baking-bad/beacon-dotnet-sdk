// ReSharper disable ArgumentsStyleNamedExpression

namespace Matrix.Sdk.Core.Infrastructure.Services
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
            byte[] loginDigest = MatrixCryptographyService.GenerateLoginDigest();
            string hexSignature = MatrixCryptographyService.GenerateHexSignature(loginDigest, keyPair.PrivateKey);
            string hexPublicKey = MatrixCryptographyService.ToHexString(keyPair.PublicKey);
            string hexId = MatrixCryptographyService.GenerateHexId(keyPair.PublicKey);

            var password = $"ed:{hexSignature}:{hexPublicKey}";
            string deviceId = hexPublicKey;

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

            return await CreateHttpClient()
                .PostAsJsonAsync<LoginResponse>($"{RequestUri}/login", model, cancellationToken);
        }
    }
}