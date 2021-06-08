namespace MatrixSdk.Services
{
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Dto;
    using Dto.Event;
    using Dto.Room.Sync;
    using Extensions;

    public class MatrixEventService
    {
        private const string RequestUri = "_matrix/client/r0";
        private readonly IHttpClientFactory httpClientFactory;

        public MatrixEventService(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        public async Task<SyncResponse> SyncAsync(string accessToken, CancellationToken cancellationToken, ulong? timeout = null, string? nextBatch = null)
        {
            var httpClient = httpClientFactory.CreateClient(Constants.Matrix);
            httpClient.AddBearerToken(accessToken);
           
            var uri = new Uri(httpClient.BaseAddress + $"{RequestUri}/sync");

            if (timeout != null)
                uri = uri.AddParameter("timeout", timeout.ToString());
            
            if (nextBatch != null)
                uri = uri.AddParameter("since", nextBatch);
           
            return await httpClient.GetAsJsonAsync<SyncResponse>(uri.ToString(), cancellationToken);
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