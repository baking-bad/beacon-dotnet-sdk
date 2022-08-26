namespace Beacon.Sdk.Core.Domain.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Entities;

    public interface IMatrixSyncRepository
    {
        Task<MatrixSyncEntity> CreateOrUpdateAsync(string nextBatch);
        
        Task<MatrixSyncEntity?> TryReadAsync();
        
        Task<List<MatrixSyncEntity>> ReadAllAsync();
    }
}