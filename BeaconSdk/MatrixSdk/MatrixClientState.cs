namespace MatrixSdk
{
    using Dto;

    public class MatrixClientState
    {
        public string? AccessToken { get; set; }

        public ulong Timeout { get; set; }

        public string? NextBatch { get; set; }
        
        public Rooms? Rooms { get; set; }
        
        public ulong TransactionNumber { get; set; }
    }
}