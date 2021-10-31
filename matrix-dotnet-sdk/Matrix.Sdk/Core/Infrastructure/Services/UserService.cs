// ReSharper disable ArgumentsStyleNamedExpression

namespace Matrix.Sdk.Core.Infrastructure.Services
{
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Domain.Services;
    using Dto.Login;
    using Extensions;
    using Sodium;

    public class UserService : BaseApiService
    {
        private readonly ICryptographyService _cryptographyService;

        public UserService(IHttpClientFactory httpClientFactory, ICryptographyService cryptographyService) : base(
            httpClientFactory)
        {
            _cryptographyService = cryptographyService;
        }

        public async Task<LoginResponse> LoginAsync(Uri baseAddress, KeyPair keyPair,
            CancellationToken cancellationToken)
        {
            byte[] loginDigest = _cryptographyService.GenerateLoginDigest();
            string hexSignature = _cryptographyService.GenerateHexSignature(loginDigest, keyPair.PrivateKey);
            string publicKeyHex = _cryptographyService.ToHexString(keyPair.PublicKey);
            string hexId = _cryptographyService.GenerateHexId(keyPair.PublicKey);

            var password = $"ed:{hexSignature}:{publicKeyHex}";
            string deviceId = publicKeyHex;

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

            HttpClient httpClient = CreateHttpClient(baseAddress);

            var path = $"{ResourcePath}/login";

            return await httpClient.PostAsJsonAsync<LoginResponse>(path, model, cancellationToken);
        }

        public async Task<LoginResponse> LoginAsync(Uri baseAddress, string user, string password, string deviceId,
            CancellationToken cancellationToken)
        {
            var model = new LoginRequest
            (
                new Identifier
                (
                    "m.id.user",
                    user
                ),
                password,
                deviceId,
                "m.login.password"
            );

            HttpClient httpClient = CreateHttpClient(baseAddress);

            var path = $"{ResourcePath}/login";

            return await httpClient.PostAsJsonAsync<LoginResponse>(path, model, cancellationToken);
        }
    }
}