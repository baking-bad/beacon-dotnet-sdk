namespace Matrix.Sdk.Clients
{
    using System.Net.Http;
    using Core.Infrastructure.Extensions;

    public abstract class BaseApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;

        protected BaseApiClient(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        protected abstract string RequestUri { get; }

        protected HttpClient CreateHttpClient(string accessToken)
        {
            HttpClient httpClient = _httpClientFactory.CreateClient(Constants.Matrix);
            httpClient.AddBearerToken(accessToken);

            return httpClient;
        }

        protected HttpClient CreateHttpClient() => _httpClientFactory.CreateClient(Constants.Matrix);
    }
}