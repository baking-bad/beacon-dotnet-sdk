namespace Beacon.Sdk.Core.Domain.Entities
{
    using LiteDB;

    public class SeedEntity
    {
        [BsonId] public string Seed { get; set; }
    }
}