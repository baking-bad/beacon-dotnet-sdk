namespace Beacon.Sdk.Core.Domain.Entities
{
    public class MatrixSyncEntity
    {
        public long Id { get; set; }
        
        public string NextBatch { get; set; }
    }
}