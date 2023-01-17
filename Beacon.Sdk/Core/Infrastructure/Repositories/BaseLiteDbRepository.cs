namespace Beacon.Sdk.Core.Infrastructure.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using LiteDB;
    using Microsoft.Extensions.Logging;

    public abstract class BaseLiteDbRepository<T>
    {
        private readonly string _connectionString;
        private readonly ILogger<BaseLiteDbRepository<T>> _logger;
        private readonly object _syncRoot = new();

        protected BaseLiteDbRepository(ILogger<BaseLiteDbRepository<T>> logger, RepositorySettings settings)
        {
            _logger = logger;
            _connectionString = settings.ConnectionString;
        }

        protected Task InConnectionAction(string collectionName, Action<LiteCollection<T>> func)
        {
            try
            {
                lock (_syncRoot)
                {
                    using var db = new LiteDatabase(_connectionString);
                    LiteCollection<T> col = db.GetCollection<T>(collectionName);

                    func(col);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "error in repository");
            }

            return Task.CompletedTask;
        }

        protected Task<T> InConnection(string collectionName, Func<LiteCollection<T>, Task<T>> func)
        {
            try
            {
                lock (_syncRoot)
                {
                    using var db = new LiteDatabase(_connectionString);
                    LiteCollection<T> col = db.GetCollection<T>(collectionName);

                    return func(col);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "error in repository");

                throw;
            }

            //return Task.FromResult<T>(default!);
        }

        protected Task InConnection(string collectionName, Action<LiteDatabase, LiteCollection<T>> func)
        {
            try
            {
                lock (_syncRoot)
                {
                    using var db = new LiteDatabase(_connectionString);
                    LiteCollection<T> col = db.GetCollection<T>(collectionName);

                    func(db, col);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "error in repository");
            }

            return Task.CompletedTask;
        }

        protected Task<T?> InConnectionNullable(string collectionName, Func<LiteCollection<T>, Task<T?>> func)
        {
            try
            {
                lock (_syncRoot)
                {
                    using var db = new LiteDatabase(_connectionString);
                    LiteCollection<T> col = db.GetCollection<T>(collectionName);

                    return func(col);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "error in repository");
            }

            return Task.FromResult<T?>(default);
        }

        protected Task<T[]?> InConnectionNullable(string collectionName, Func<LiteCollection<T>, Task<T[]?>> func)
        {
            try
            {
                lock (_syncRoot)
                {
                    using var db = new LiteDatabase(_connectionString);
                    LiteCollection<T> col = db.GetCollection<T>(collectionName);

                    return func(col);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "error in repository");
            }

            return Task.FromResult<T[]?>(default);
        }

        protected Task<List<T>> InConnection(string collectionName, Func<LiteCollection<T>, Task<List<T>>> func)
        {
            try
            {
                lock (_syncRoot)
                {
                    using var db = new LiteDatabase(_connectionString);
                    LiteCollection<T> col = db.GetCollection<T>(collectionName);

                    return func(col);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "error in repository");
            }

            return Task.FromResult<List<T>>(default!);
        }
        
        protected Task DropAsync(string collectionName) => InConnection(collectionName, (db, col) =>
        {
            db.DropCollection(col.Name);
        });
    }
}