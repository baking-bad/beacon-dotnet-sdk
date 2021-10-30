namespace Matrix.Sdk.Core.Domain.Services
{
    using System;
    using System.Threading;
    using MatrixRoom;

    public interface ISyncService
    {
        MatrixRoom[] InvitedRooms { get; }

        MatrixRoom[] JoinedRooms { get; }

        MatrixRoom[] LeftRooms { get; }

        void Start(Uri nodeAddress, string accessToken, CancellationToken cancellationToken,
            Action<SyncBatch> onNewSyncBatch);

        void Stop();

        void UpdateMatrixRoom(string roomId, MatrixRoom matrixRoom);

        MatrixRoom? GetMatrixRoom(string roomId);
    }
}