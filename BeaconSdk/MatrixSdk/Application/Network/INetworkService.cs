namespace MatrixSdk.Application.Network
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Domain;
    using Infrastructure.Dto.Login;
    using Infrastructure.Dto.Sync;
    using Sodium;

    public interface INetworkService
    {
        Task<LoginResponse> LoginAsync(CancellationToken cancellationToken, KeyPair keyPair);

        Task<SyncResponse> SyncAsync(string accessToken, CancellationToken cancellationToken, ulong? timeout = null,
            string? nextBatch = null);

        Task<MatrixRoom> CreateTrustedPrivateRoomAsync(ClientStateManager stateManager, CancellationToken cancellationToken,
            string[]? invitedUserIds = null);

        Task<MatrixRoom> JoinTrustedPrivateRoomAsync(ClientStateManager stateManager, CancellationToken cancellationToken, string roomId);

        Task<string?> SendMessageAsync(ClientStateManager stateManager, CancellationToken cancellationToken, string roomId, string message);

        Task<List<string>> GetJoinedRoomsIdsAsync(string accessToken, CancellationToken cancellationToken);

        Task LeaveRoomAsync(string accessToken, CancellationToken cancellationToken, string roomId);
    }
}