namespace MatrixSdk
{
    using System.Collections.Concurrent;

    public class MatrixClientState
    {
        public string? UserId { get; init; }

        public string? AccessToken { get; set; }

        public ulong Timeout { get; set; }

        public string? NextBatch { get; set; }

        public ulong TransactionNumber { get; set; }

        public ConcurrentDictionary<string, MatrixRoom> MatrixRooms { get; init; }
    }
}