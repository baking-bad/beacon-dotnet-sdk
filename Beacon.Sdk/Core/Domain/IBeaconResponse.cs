namespace Beacon.Sdk.Core.Domain
{
    using Beacon;

    public interface IBeaconResponse
    {
        public string Id { get; }

        public BeaconMessageType Type { get; }
    }
}