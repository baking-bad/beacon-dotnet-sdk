namespace Matrix.Sdk.Core.Infrastructure.Services
{
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Dto.Room.Create;
    using Dto.Room.Join;
    using Dto.Room.Joined;
    using Extensions;

    public class RoomService : BaseApiService
    {
        public RoomService(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        {
        }

        public async Task<CreateRoomResponse> CreateRoomAsync(Uri baseAddress, string accessToken, string[]? members,
            CancellationToken cancellationToken)
        {
            var model = new CreateRoomRequest
            (
                Invite: members,
                Preset: Preset.TrustedPrivateChat,
                IsDirect: true
            );

            HttpClient httpClient = CreateHttpClient(baseAddress, accessToken);

            var path = $"{ResourcePath}/createRoom";

            return await httpClient.PostAsJsonAsync<CreateRoomResponse>(path, model, cancellationToken);
        }

        public async Task<JoinRoomResponse> JoinRoomAsync(Uri baseAddress, string accessToken, string roomId,
            CancellationToken cancellationToken)
        {
            HttpClient httpClient = CreateHttpClient(baseAddress, accessToken);

            var path = $"{ResourcePath}/rooms/{roomId}/join";

            return await httpClient.PostAsJsonAsync<JoinRoomResponse>(path, null, cancellationToken);
        }


        public async Task<JoinedRoomsResponse> GetJoinedRoomsAsync(Uri baseAddress, string accessToken,
            CancellationToken cancellationToken)
        {
            HttpClient httpClient = CreateHttpClient(baseAddress, accessToken);

            var path = $"{ResourcePath}/joined_rooms";

            return await httpClient.GetAsJsonAsync<JoinedRoomsResponse>(path, cancellationToken);
        }

        public async Task LeaveRoomAsync(Uri baseAddress, string accessToken, string roomId,
            CancellationToken cancellationToken)
        {
            HttpClient httpClient = CreateHttpClient(baseAddress, accessToken);

            var path = $"{ResourcePath}/rooms/{roomId}/leave";

            await httpClient.PostAsync(path, cancellationToken);
        }
    }
}