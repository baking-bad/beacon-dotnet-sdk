using System;
using System.Collections.Concurrent;
using System.IO;

using LiteDB;

namespace Beacon.Sdk.Core.Infrastructure
{
    public class LiteDbConnectionPool : IDisposable, ILiteDbConnectionPool
    {
        private readonly ConcurrentDictionary<string, LiteDatabase> _dbs;
        private bool _disposed;

        public LiteDbConnectionPool()
        {
            _dbs = new ConcurrentDictionary<string, LiteDatabase>();
        }

        public ILiteDatabase OpenConnection(ConnectionString connectionString, BsonMapper? mapper = null)
        {
            var fullPath = Path.GetFullPath(connectionString.Filename);

            LiteDatabase? newDb = null;

            try
            {
                var db = _dbs.GetOrAdd(fullPath, path =>
                {
                    newDb = new LiteDatabase(connectionString, mapper);
                    return newDb;
                });

                if (db != newDb && newDb != null)
                {
                    newDb.Dispose();
                }

                return db;
            }
            catch (Exception ex)
            {
                if (!_dbs.TryGetValue(fullPath, out var db))
                    throw ex; // connection not found after another attempt to get => throw exception when creating connection

                return db;
            }
        }

        public void CloseConnection(string fileName)
        {
            var fullPath = Path.GetFullPath(fileName);

            if (_dbs.TryRemove(fullPath, out var db))
            {
                db.Dispose();
            }
        }

        public void CloseAllConnections()
        {
            var keysSnapshot = _dbs.Keys;

            foreach (var fullFilePath in keysSnapshot)
            {
                if (_dbs.TryRemove(fullFilePath, out var db))
                {
                    db.Dispose();
                }
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    CloseAllConnections();
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}