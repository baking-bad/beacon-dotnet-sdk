namespace Beacon.Sdk.Core.Infrastructure.Repositories
{
    using System;
    using System.Threading.Tasks;
    using LiteDB;
    using Microsoft.Extensions.Logging;

    public abstract class BaseLiteDbRepository<T>
    {
        private readonly string _connectionString;
        private readonly object _syncRoot = new();
        private readonly ILogger<BaseLiteDbRepository<T>> _logger;

        protected BaseLiteDbRepository(ILogger<BaseLiteDbRepository<T>> logger, RepositorySettings settings)
        {
            _logger = logger;
            _connectionString = settings.ConnectionString;

            // BsonMapper.Global.RegisterType<HexString>
            // (
            //     serialize: hexString => hexString.Value,
            //     deserialize: bson => new HexString(bson.AsArray));
            // }
        }

        protected Task<T> InConnection(Func<ILiteCollection<T>, Task<T>> func)
        {
            try
            {
                lock (_syncRoot)
                {
                    using var db = new LiteDatabase(_connectionString);
                    ILiteCollection<T> col = db.GetCollection<T>(nameof(T));

                    return func(col);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "error in repository");
            }

            return new Task<T>(() => default!);
        }

        protected Task<T?> InConnectionNullable(Func<ILiteCollection<T>, Task<T?>> func)
        {
            try
            {
                lock (_syncRoot)
                {
                    using var db = new LiteDatabase(_connectionString);
                    ILiteCollection<T> col = db.GetCollection<T>(nameof(T));

                    return func(col);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "error in repository");
            }

            return new Task<T?>(() => default);
        }
        
        
        protected Task<T[]?> InConnectionNullable(Func<ILiteCollection<T>, Task<T[]?>> func)
        {
            try
            {
                lock (_syncRoot)
                {
                    using var db = new LiteDatabase(_connectionString);
                    ILiteCollection<T> col = db.GetCollection<T>(nameof(T));

                    return func(col);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "error in repository");
            }

            return new Task<T[]?>(() => default);
        }
    }
}