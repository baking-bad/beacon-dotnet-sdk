namespace Matrix.Sdk.Core.Domain.Services
{
    using System;
    using MatrixRoom;

    public interface IPollingService : IDisposable
    {
        public event EventHandler<SyncBatchEventArgs> OnSyncBatchReceived;
        
        MatrixRoom[] InvitedRooms { get; }

        MatrixRoom[] JoinedRooms { get; }

        MatrixRoom[] LeftRooms { get; }

        void Init(Uri nodeAddress, string accessToken);

        void Start();

        void Stop();

        void UpdateMatrixRoom(string roomId, MatrixRoom matrixRoom);

        MatrixRoom? GetMatrixRoom(string roomId);
    }
}