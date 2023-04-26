namespace Beacon.Sdk.Core.Infrastructure.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LiteDB;
    using Microsoft.Extensions.Logging;

    public abstract class BaseLiteDbRepository<T>
    {
        private readonly ILogger<BaseLiteDbRepository<T>> _logger;
        private readonly ILiteDatabase _db;

        protected BaseLiteDbRepository(ILiteDbConnectionPool connectionPool, ILogger<BaseLiteDbRepository<T>> logger, RepositorySettings settings)
        {
            _logger = logger;
            _db = connectionPool.OpenConnection(new ConnectionString(settings.ConnectionString));
        }

        protected Task InConnectionAction(string collectionName, Action<ILiteCollection<T>> func)
        {
            try
            {
                ILiteCollection<T> col = _db.GetCollection<T>(collectionName);
                func(col);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "error in repository");
            }

            return Task.CompletedTask;
        }

        protected Task<T> InConnection(string collectionName, Func<ILiteCollection<T>, Task<T>> func)
        {
            try
            {
                ILiteCollection<T> col = _db.GetCollection<T>(collectionName);
                return func(col);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "error in repository");
                throw;
            }
        }

        protected Task InConnection(string collectionName, Action<ILiteDatabase, ILiteCollection<T>> func)
        {
            try
            {
                ILiteCollection<T> col = _db.GetCollection<T>(collectionName);
                func(_db, col);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "error in repository");
            }

            return Task.CompletedTask;
        }

        protected Task<T?> InConnectionNullable(string collectionName, Func<ILiteCollection<T>, Task<T?>> func)
        {
            try
            {
                ILiteCollection<T> col = _db.GetCollection<T>(collectionName);
                return func(col);

            }
            catch (Exception e)
            {
                _logger.LogError(e, "error in repository");
            }

            return Task.FromResult<T?>(default);
        }

        protected Task<T[]?> InConnectionNullable(string collectionName, Func<ILiteCollection<T>, Task<T[]?>> func)
        {
            try
            {
                ILiteCollection<T> col = _db.GetCollection<T>(collectionName);
                return func(col);

            }
            catch (Exception e)
            {
                _logger.LogError(e, "error in repository");
            }

            return Task.FromResult<T[]?>(default);
        }

        protected Task<List<T>> InConnection(string collectionName, Func<ILiteCollection<T>, Task<List<T>>> func)
        {
            try
            {
                ILiteCollection<T> col = _db.GetCollection<T>(collectionName);
                return func(col);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "error in repository");
            }

            return Task.FromResult<List<T>>(default!);
        }

        protected Task DropAsync(string collectionName) =>
            InConnection(collectionName, (db, col) => { db.DropCollection(col.Name); });
    }
}