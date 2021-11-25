namespace Beacon.Sdk.Beacon
{
    using LiteDB;

    // public record AppMetadata(string SenderId, string Name, string? Icon);

    public class AppMetadata
    {
        [BsonId] public string SenderId { get; set; }

        public string Name { get; set; }

        public string? Icon { get; set; }
    }
}