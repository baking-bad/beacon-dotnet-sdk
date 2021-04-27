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
        
        public async Task<EventResponse> SendEventAsync(string accessToken, string roomId, string txnId, string msgtype)
        {
            var httpClient = httpClientFactory.CreateClient(Constants.Matrix);
            httpClient.AddBearerToken(accessToken);

            var type = "m.room.message";

            var model = new MessageEvent(msgtype);
            
            return await httpClient.PutAsJsonAsync<EventResponse>($"{RequestUri}/rooms/{roomId}/send/{type}/{txnId}", model);
        }
    }
}