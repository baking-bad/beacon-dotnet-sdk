using LiteDB;

namespace Beacon.Sdk.Core.Infrastructure
{
    public interface ILiteDbConnectionPool
    {
        void CloseAllConnections();
        void CloseConnection(string fileName);
        ILiteDatabase OpenConnection(ConnectionString connectionString, BsonMapper? mapper = null);
    }
}