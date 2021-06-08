namespace MatrixSdk.Dto.Room.Sync
{
    public record SyncResponse(string NextBatch, Rooms Rooms)
    {
        public string NextBatch { get; } = NextBatch;
        public Rooms Rooms { get; } = Rooms;
    }
}