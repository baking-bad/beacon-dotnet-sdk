namespace Beacon.Sdk.Core.Domain
{
    using Beacon;

    public interface IBeaconRequest
    {
        public string Id { get; }

        public string SenderId { get; }

        public string Version { get; }

        public BeaconMessageType Type { get; }
    }
}