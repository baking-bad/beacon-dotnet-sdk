namespace MatrixSdk.Services
{
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Dto.Event;
    using Dto.Room.Sync;
    using Extensions;

    public class EventService
    {
        private const string RequestUri = "_matrix/client/r0";
        private readonly IHttpClientFactory httpClientFactory;
        private HttpClient CreateHttpClient(string accessToken)
        {
            var httpClient = httpClientFactory.CreateClient(MatrixConstants.Matrix);
            httpClient.AddBearerToken(accessToken);

            return httpClient;
        }
        
        public EventService(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        public async Task<SyncResponse> SyncAsync(string accessToken, CancellationToken cancellationToken, ulong? timeout = null, string? nextBatch = null)
        {
            var uri = new Uri($"{MatrixConstants.BaseAddress}{RequestUri}/sync");

            if (timeout != null)
                uri = uri.AddParameter("timeout", timeout.ToString());

            if (nextBatch != null)
                uri = uri.AddParameter("since", nextBatch);

            return await CreateHttpClient(accessToken).GetAsJsonAsync<SyncResponse>(uri.ToString(), cancellationToken);
        }

        public async Task<EventResponse> SendEventAsync(string accessToken, string roomId, string txnId, string msgtype)
        {
            var type = "m.room.message";
            var model = new MessageEvent(msgtype);

            return await CreateHttpClient(accessToken).PutAsJsonAsync<EventResponse>($"{RequestUri}/rooms/{roomId}/send/{type}/{txnId}", model);
        }
    }
}