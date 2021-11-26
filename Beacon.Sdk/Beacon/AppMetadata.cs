namespace Beacon.Sdk.Beacon
{
    using LiteDB;

    public class AppMetadata
    {
        [BsonId] public string SenderId { get; set; }

        public string Name { get; set; }

        public string? Icon { get; set; }
    }
}