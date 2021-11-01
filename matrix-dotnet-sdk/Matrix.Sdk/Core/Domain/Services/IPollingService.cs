namespace Matrix.Sdk.Core.Domain.Services
{
    using System;
    using MatrixRoom;

    public interface IPollingService
    {
        MatrixRoom[] InvitedRooms { get; }

        MatrixRoom[] JoinedRooms { get; }

        MatrixRoom[] LeftRooms { get; }

        void Start(Uri nodeAddress, string accessToken, Action<SyncBatch> onNewSyncBatch);

        void Stop();

        void UpdateMatrixRoom(string roomId, MatrixRoom matrixRoom);

        MatrixRoom? GetMatrixRoom(string roomId);
    }
}