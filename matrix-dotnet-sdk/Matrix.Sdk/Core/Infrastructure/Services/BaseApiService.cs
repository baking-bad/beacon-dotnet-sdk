namespace Matrix.Sdk.Core.Infrastructure.Services
{
    using System;
    using System.Net.Http;
    using Extensions;

    public abstract class BaseApiService
    {
        // see: https://github.com/dotnet/aspnetcore/issues/28385#issuecomment-853766480
        private readonly IHttpClientFactory _httpClientFactory;

        protected BaseApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        protected virtual string ResourcePath => "_matrix/client/r0";

        /// <summary>
        ///     Creates HttpClient
        /// </summary>
        /// <param name="baseAddress">Address of a Matrix node.</param>
        /// <param name="accessToken">User access token.</param>
        /// <returns></returns>
        protected HttpClient CreateHttpClient(Uri? baseAddress = null, string? accessToken = null)
        {
            HttpClient httpClient = _httpClientFactory.CreateClient(Constants.Matrix);

            if (accessToken != null)
                httpClient.AddBearerToken(accessToken);

            if (baseAddress != null) httpClient.BaseAddress = baseAddress;

            return httpClient;
        }
    }
}