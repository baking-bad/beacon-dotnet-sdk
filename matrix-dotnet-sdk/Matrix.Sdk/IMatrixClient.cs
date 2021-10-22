namespace Matrix.Sdk
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core;
    using Core.Domain;
    using Core.Domain.Room;
    using Sodium;

    public interface IMatrixClient
    {
        MatrixEventNotifier<List<BaseRoomEvent>> MatrixEventNotifier { get; }
        
        MatrixRoom[] InvitedRooms { get; }

        MatrixRoom[] JoinedRooms { get; }

        MatrixRoom[] LeftRooms { get; }

        Task StartAsync(KeyPair keyPair);

        void Stop();

        Task<MatrixRoom> CreateTrustedPrivateRoomAsync(string[] invitedUserIds = null);

        Task<MatrixRoom> JoinTrustedPrivateRoomAsync(string roomId);

        Task<string> SendMessageAsync(string roomId, string message);

        Task<List<string>> GetJoinedRoomsIdsAsync();

        Task LeaveRoomAsync(string roomId);
    }
}