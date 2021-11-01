namespace Matrix.Sdk.Core.Domain.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Infrastructure.Dto.Login;
    using MatrixRoom;

    public interface INetworkService
    {
        Task<LoginResponse> LoginAsync(Uri baseAddress, string user, string password, string deviceId,
            CancellationToken cancellationToken);

        Task<MatrixRoom> CreateTrustedPrivateRoomAsync(Uri baseAddress, string accessToken, string[] invitedUserIds,
            CancellationToken cancellationToken);

        Task<MatrixRoom> JoinTrustedPrivateRoomAsync(Uri baseAddress, string accessToken, string roomId,
            CancellationToken cancellationToken);

        Task<string> SendMessageAsync(Uri baseAddress, string accessToken, string roomId, string transactionId,
            string message, CancellationToken cancellationToken);

        Task<List<string>> GetJoinedRoomsIdsAsync(Uri baseAddress, string accessToken,
            CancellationToken cancellationToken);

        Task LeaveRoomAsync(Uri baseAddress, string accessToken, string roomId,
            CancellationToken cancellationToken);
    }
}