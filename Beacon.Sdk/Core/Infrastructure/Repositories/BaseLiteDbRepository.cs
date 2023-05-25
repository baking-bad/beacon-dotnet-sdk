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
        private readonly ILiteDbConnectionPool _connectionPool;
        private readonly RepositorySettings _settings;

        protected BaseLiteDbRepository(
            ILiteDbConnectionPool connectionPool,
            ILogger<BaseLiteDbRepository<T>> logger,
            RepositorySettings settings)
        {
            _connectionPool = connectionPool ?? throw new ArgumentNullException(nameof(connectionPool));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _logger = logger;
        }

        protected Task InConnectionAction(string collectionName, Action<ILiteCollection<T>> func)
        {
            try
            {
                var db = _connectionPool.OpenConnection(new ConnectionString(_settings.ConnectionString));
                var col = db.GetCollection<T>(collectionName);
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
                var db = _connectionPool.OpenConnection(new ConnectionString(_settings.ConnectionString));
                var col = db.GetCollection<T>(collectionName);
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
                var db = _connectionPool.OpenConnection(new ConnectionString(_settings.ConnectionString));
                var col = db.GetCollection<T>(collectionName);
                func(db, col);
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
                var db = _connectionPool.OpenConnection(new ConnectionString(_settings.ConnectionString));
                var col = db.GetCollection<T>(collectionName);
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
                var db = _connectionPool.OpenConnection(new ConnectionString(_settings.ConnectionString));
                var col = db.GetCollection<T>(collectionName);
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
                var db = _connectionPool.OpenConnection(new ConnectionString(_settings.ConnectionString));
                var col = db.GetCollection<T>(collectionName);
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