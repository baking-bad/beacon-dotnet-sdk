// ReSharper disable ArgumentsStyleNamedExpression

namespace Matrix.Sdk.Clients
{
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Core.Infrastructure.Dto.Login;
    using Core.Infrastructure.Extensions;
    using Core.Infrastructure.Services;
    using Sodium;

    public class UserClient : BaseApiClient
    {
        public UserClient(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        {
        }

        protected override string RequestUri => "_matrix/client/r0";

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