namespace MatrixSdk
{
    using Dto;
    using Dto.Room.Sync;

    public class MatrixClientState
    {
        public string? UserId { get; set; }
        
        public string? AccessToken { get; set; }

        public ulong Timeout { get; set; }

        public string? NextBatch { get; set; }
        
        public Rooms? Rooms { get; set; }
        
        public ulong TransactionNumber { get; set; }
    }
}