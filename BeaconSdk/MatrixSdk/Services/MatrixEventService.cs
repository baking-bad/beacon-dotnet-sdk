namespace MatrixSdk.Services
{
    using System.Net.Http;
    using System.Threading.Tasks;

    public class MatrixEventService
    {
        private readonly IHttpClientFactory httpClientFactory;

        public MatrixEventService(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        public async Task<int> SyncAsync()
        {
            
            return await Task.FromResult(42);
        }
    }
}