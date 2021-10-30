namespace Matrix.Sdk.Core.Infrastructure.Services
{
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Dto.Event;
    using Dto.Sync;
    using Extensions;

    public class EventService : BaseApiService
    {
        public EventService(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        {
        }

        public async Task<SyncResponse> SyncAsync(Uri baseAddress, string accessToken,
            CancellationToken cancellationToken,
            ulong? timeout = null, string? nextBatch = null)
        {
            var uri = new Uri($"{Constants.BaseAddress}{ResourcePath}/sync");

            if (timeout != null)
                uri = uri.AddParameter("timeout", timeout.ToString());

            if (nextBatch != null)
                uri = uri.AddParameter("since", nextBatch);

            HttpClient httpClient = CreateHttpClient(baseAddress, accessToken);

            return await httpClient.GetAsJsonAsync<SyncResponse>(uri.ToString(), cancellationToken);
        }

        public async Task<EventResponse> SendMessageAsync(Uri baseAddress, string accessToken,
            CancellationToken cancellationToken,
            string roomId, string transactionId,
            string message)
        {
            const string eventType = "m.room.message";
            var model = new MessageEvent(MessageType.Text, message);

            HttpClient httpClient = CreateHttpClient(baseAddress, accessToken);

            var path = $"{ResourcePath}/rooms/{roomId}/send/{eventType}/{transactionId}";

            return await httpClient.PutAsJsonAsync<EventResponse>(path, model, cancellationToken);
        }
    }
}