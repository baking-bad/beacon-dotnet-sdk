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

            // var t = new MatrixSyncResponse();
            // // t.MatrixRooms.Invite[0].InviteState.Events[0].
            // // var k = t.Rooms.Join[0].TimeLine.Events[0].Sender;
            // // Dto.Sync.Rooms a = t.Rooms;
            // // Dto.Sync.Rooms? b = t.Rooms;
            // return await Task.FromResult(42);
        }
    }
}