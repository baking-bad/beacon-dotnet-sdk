namespace Matrix.Sdk.Core.Domain.Services
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Infrastructure.Dto.Login;
    using MatrixRoom;
    using Network;
    using LoginRequest = Network.LoginRequest;

    public interface INetworkService
    {
        Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken);

        Task<MatrixRoom> CreateTrustedPrivateRoomAsync(CreateTrustedPrivateRoomRequest request,
            CancellationToken cancellationToken);

        Task<MatrixRoom> JoinTrustedPrivateRoomAsync(JoinTrustedPrivateRoomRequest request,
            CancellationToken cancellationToken);

        Task<string> SendMessageAsync(SendMessageRequest request, CancellationToken cancellationToken);

        Task<List<string>> GetJoinedRoomsIdsAsync(GetJoinedRoomsIdsRequest request,
            CancellationToken cancellationToken);

        Task LeaveRoomAsync(LeaveRoomRequest request, CancellationToken cancellationToken);
    }
}