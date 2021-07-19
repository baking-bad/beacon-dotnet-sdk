namespace MatrixSdk.Domain
{
    using System;
    using System.Collections.Concurrent;

    public class MatrixClientState
    {
        public Guid Id { get; init; }
        public string? UserId { get; set; }

        public string? AccessToken { get; set; }

        public ulong Timeout { get; set; }

        public string? NextBatch { get; set; }

        public ulong TransactionNumber { get; set; }

        public ConcurrentDictionary<string, MatrixRoom> MatrixRooms { get; init; }
    }
}