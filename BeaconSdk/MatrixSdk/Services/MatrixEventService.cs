namespace MatrixSdk.Services
{
    using System.Net.Http;
    using System.Threading.Tasks;
    using Dto;
    using Extensions;

    public class MatrixEventService
    {
        private const string RequestUri = "_matrix/client/r0";
        private readonly IHttpClientFactory httpClientFactory;

        public MatrixEventService(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        public async Task<MatrixSyncResponse> SyncAsync(string accessToken)
        {
            var httpClient = httpClientFactory.CreateClient(Constants.Matrix);
            httpClient.AddBearerToken(accessToken);

            return await httpClient.GetAsJsonAsync<MatrixSyncResponse>($"{RequestUri}/sync");
        }
    }
}