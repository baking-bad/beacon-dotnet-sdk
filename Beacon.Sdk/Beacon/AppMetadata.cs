namespace Beacon.Sdk.Beacon
{
    using LiteDB;

    public class AppMetadata
    {
        public long Id { get; set; }
        
        public string SenderId { get; set; }

        public string Name { get; set; }

        public string? Icon { get; set; }
    }
}