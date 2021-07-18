namespace MatrixSdk.Infrastructure.Services
{
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Dto.Event;
    using Dto.Sync;
    using Extensions;

    public class EventService
    {
        private const string RequestUri = "_matrix/client/r0";
        private readonly IHttpClientFactory httpClientFactory;

        public EventService(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        private HttpClient CreateHttpClient(string accessToken)
        {
            var httpClient = httpClientFactory.CreateClient(MatrixApiConstants.Matrix);
            httpClient.AddBearerToken(accessToken);

            return httpClient;
        }

        public async Task<SyncResponse> SyncAsync(string accessToken, CancellationToken cancellationToken, ulong? timeout = null, string? nextBatch = null)
        {
            var uri = new Uri($"{MatrixApiConstants.BaseAddress}{RequestUri}/sync");

            if (timeout != null)
                uri = uri.AddParameter("timeout", timeout.ToString());

            if (nextBatch != null)
                uri = uri.AddParameter("since", nextBatch);

            return await CreateHttpClient(accessToken).GetAsJsonAsync<SyncResponse>(uri.ToString(), cancellationToken);
        }

        public async Task<EventResponse> SendMessageAsync(string accessToken, string roomId, string transactionId, string message)
        {
            // var eventType = InstantMessagingEventType.Message.ToString();
            var eventType = "m.room.message";
            var model = new MessageEvent(MessageType.Text, message);

            return await CreateHttpClient(accessToken)
                .PutAsJsonAsync<EventResponse>($"{RequestUri}/rooms/{roomId}/send/{eventType}/{transactionId}", model);
        }
    }
}