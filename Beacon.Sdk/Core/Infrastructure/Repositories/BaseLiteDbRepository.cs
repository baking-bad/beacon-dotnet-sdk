namespace Beacon.Sdk.Core.Infrastructure.Repositories
{
    using System;
    using LiteDB;
    using Utils;

    public abstract class BaseLiteDbRepository
    {
        protected readonly string ConnectionString;

        protected BaseLiteDbRepository(RepositorySettings settings)
        {
            ConnectionString = settings.ConnectionString;
            
            // BsonMapper.Global.RegisterType<HexString>
            // (
            //     serialize: hexString => hexString.Value,
            //     deserialize: bson => new HexString(bson.AsArray));
        }
    }
}