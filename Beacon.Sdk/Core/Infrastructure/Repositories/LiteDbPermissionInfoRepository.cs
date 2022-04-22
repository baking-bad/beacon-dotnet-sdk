namespace Beacon.Sdk.Core.Infrastructure.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Domain.Entities;
    using Domain.Interfaces.Data;
    using Microsoft.Extensions.Logging;

    public class LiteDbPermissionInfoRepository : BaseLiteDbRepository<PermissionInfo>, IPermissionInfoRepository
    {
        private const string CollectionName = "PermissionInfo";
        
        public LiteDbPermissionInfoRepository(ILogger<LiteDbPermissionInfoRepository> logger,
            RepositorySettings settings) : base(logger, settings)
        {
        }

        public Task<PermissionInfo> CreateOrUpdateAsync(PermissionInfo permissionInfo) => 
            InConnection(CollectionName,col =>
            {
                PermissionInfo? oldPermissionInfo = col.FindOne(x => x.AccountIdentifier == permissionInfo.AccountIdentifier);

                if (oldPermissionInfo != null)
                    col.Update(permissionInfo);
                else
                    col.Insert(permissionInfo);
                
                col.EnsureIndex(x => x.AccountIdentifier);
                
                return Task.FromResult(permissionInfo);
            });

        public Task<PermissionInfo?> TryReadAsync(string accountIdentifier) => 
            InConnectionNullable(CollectionName,col =>
            {
                // PermissionInfo? permissionInfo =
                //     col.Query().Where(x => x.AccountIdentifier == accountIdentifier).FirstOrDefault();

                PermissionInfo? permissionInfo = col.FindOne(x => x.AccountIdentifier == accountIdentifier);

                return Task.FromResult(permissionInfo ?? null);
            });

        public Task<List<PermissionInfo>> ReadAllAsync() => 
            InConnection(CollectionName,col =>
            {
                var result = col.FindAll().ToList();

                return Task.FromResult(result);
            });

        public Task DeleteByAddressAsync(string address) => 
            InConnection(CollectionName,col =>
            {
                col.EnsureIndex(x => x.AccountIdentifier);

                col.Delete(x => x.Address == address);
                //PermissionInfo? permissionInfo = col.FindOne(x => x.Address == address);
                
                //if (permissionInfo != null)
                //    col.Delete(permissionInfo.Id);

                return (Task<PermissionInfo>) Task.CompletedTask;
            });
    }
}

//"  at (wrapper dynamic-method) System.Object.lambda_method(System.Runtime.CompilerServices.Closure,object,object)\n  at LiteDB.BsonMapper.DeserializeObject (System.Type type, System.Object obj, LiteDB.BsonDocument value) [0x0008b] in <9e5fde0ddc6f45bfbe6bed2535394f2f>:0 \n  at LiteDB.BsonMapper.Deserialize (System.Type type, LiteDB.BsonValue value) [0x00202] in <9e5fde0ddc6f45bfbe6bed2535394f2f>:0 \n  at LiteDB.BsonMapper.ToObject (System.Type type, LiteDB.BsonDocument doc) [0x00028] in <9e5fde0ddc6f45bfbe6bed2535394f2f>:0 \n  at LiteDB.BsonMapper.ToObject[T] (LiteDB.BsonDocument doc) [0x00000] in <9e5fde0ddc6f45bfbe6bed2535394f2f>:0 \n  at LiteDB.LiteCollection`1+<Find>d__17[T].MoveNext () [0x00090] in <9e5fde0ddc6f45bfbe6bed2535394f2f>:0 \n  at System.Collections.Generic.List`1[T].AddEnumerable (System.Collections.Generic.IEnumerable`1[T] enumerable) [0x00059] in /Users/builder/jenkins/workspace/archive-mono/2020-02/android/release/external/corefx/src/Common/src/CoreLib/System/Collections/Generic/List.cs:1108 \n  at System.Collections.Generic.List`1[T]..ctor (System.Collections.Generic.IEnumerable`1[T] collection) [0x00062] in /Users/builder/jenkins/workspace/archive-mono/2020-02/android/release/external/corefx/src/Common/src/CoreLib/System/Collections/Generic/List.cs:87 \n  at System.Linq.Enumerable.ToList[TSource] (System.Collections.Generic.IEnumerable`1[T] source) [0x0000e] in /Users/builder/jenkins/workspace/archive-mono/2020-02/android/release/external/corefx/src/System.Linq/src/System/Linq/ToCollection.cs:30 \n  at Beacon.Sdk.Core.Infrastructure.Repositories.LiteDbPermissionInfoRepository+<>c.<ReadAllAsync>b__3_0 (LiteDB.LiteCollection`1[T] col) [0x00002] in /Users/mikhailtatarenko/Documents/Code/Github/beacon-dotnet-sdk/Beacon.Sdk/Core/Infrastructure/Repositories/LiteDbPermissionInfoRepository.cs:57 "

// InConnectionNullable(col =>
// {
//     AppMetadata[]? result = col.FindAll().ToArray();
//     // AppMetadata[]? result = col.Query().ToArray();
//
//     return Task.FromResult(result ?? null);
// });