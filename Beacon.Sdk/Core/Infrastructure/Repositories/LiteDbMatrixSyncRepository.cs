using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using LiteDB;
using Microsoft.Extensions.Logging;

namespace Beacon.Sdk.Core.Infrastructure.Repositories
{
    using Domain.Entities;
    using Domain.Interfaces;

    public class LiteDbMatrixSyncRepository : BaseLiteDbRepository<MatrixSyncEntity>, IMatrixSyncRepository
    {
        private const string CollectionName = "MatrixSync";
        
        public LiteDbMatrixSyncRepository(
            ILiteDbConnectionPool connectionPool,
            ILogger<LiteDbMatrixSyncRepository> logger,
            RepositorySettings settings)
            : base(connectionPool, logger, settings)
        {
        }

        public async Task<MatrixSyncEntity> CreateOrUpdateAsync(string nextBatch)
        {
            var func = new Func<ILiteCollection<MatrixSyncEntity>, Task<MatrixSyncEntity>>(col =>
            {
                MatrixSyncEntity? matrixSyncEntity = col.FindAll().FirstOrDefault();

                if (matrixSyncEntity == null)
                {
                    matrixSyncEntity = new MatrixSyncEntity
                    {
                        NextBatch = nextBatch
                    };

                    col.Insert(matrixSyncEntity);
                }
                else
                {
                    matrixSyncEntity.NextBatch = nextBatch;

                    col.Update(matrixSyncEntity);
                }

                return Task.FromResult(matrixSyncEntity);
            });

            try
            {
                return await InConnection(CollectionName, func);
            }
            catch
            {
                await DropAsync(CollectionName);
                
                return await InConnection(CollectionName, func);
            }
        }

        public Task<MatrixSyncEntity?> TryReadAsync() =>
            InConnectionNullable(CollectionName, col =>
            {
                MatrixSyncEntity? matrixSyncEntity = col.FindAll().FirstOrDefault();

                return Task.FromResult(matrixSyncEntity ?? null);
            });

        public Task<List<MatrixSyncEntity>> ReadAllAsync() =>
            InConnection(CollectionName, col =>
            {
                var matrixSyncEntities = col.FindAll().ToList();

                return Task.FromResult(matrixSyncEntities);
            });
    }
}