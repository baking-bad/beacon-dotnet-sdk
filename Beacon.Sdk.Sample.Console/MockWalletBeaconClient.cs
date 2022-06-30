namespace Beacon.Sdk.Sample.Console
{
    using System;
    using System.Threading.Tasks;
    using Beacon;
    using Beacon.Permission;
    using Core.Domain.Entities;
    using Core.Domain.Interfaces.Data;
    using Utils;

    public record MockRequest : BaseBeaconMessage
    {
        // public string Id { get; }
        // public string SenderId { get; }
        // public string Version { get; }
        // public BeaconMessageType Type { get; }
        public MockRequest(BeaconMessageType type, string version, string id, string senderId) : base(type, version, id,
            senderId)
        {
        }

        protected MockRequest(BeaconMessageType type, string id, string senderId) : base(type, id, senderId)
        {
        }
    }

    public class MockWalletBeaconClient : IWalletBeaconClient
    {
        public HexString BeaconId { get; }

        public string AppName { get; }

        public string SenderId { get; }

        public IPermissionInfoRepository PermissionInfoRepository { get; }
        public ISeedRepository SeedRepository { get; }
        public bool LoggedIn { get; }

        public bool Connected { get; }

        public AppMetadata Metadata { get; }

        public IAppMetadataRepository AppMetadataRepository { get; }

        public event EventHandler<BeaconMessageEventArgs>? OnBeaconMessageReceived;
        public event EventHandler<DappConnectedEventArgs> OnDappConnected;

        public Task SendResponseAsync(string receiverId, BaseBeaconMessage response) =>
            throw new NotImplementedException();

        public Task InitAsync()
        {
            Console.WriteLine("Init");

            return Task.CompletedTask;
        }

        public async Task AddPeerAsync(P2PPairingRequest pairingRequest, bool sendPairingResponse = true)
        {
            await Task.Delay(100);

            // var eventArgs = new BeaconMessageEventArgs("", new BeaconBaseMessage(BeaconMessageType.acknowledge, "2", "id", "senderId"));
            // OnBeaconMessageReceived?.Invoke(this, new BeaconMessageEventArgs("", new MockRequest()));

            await Task.CompletedTask;
        }

        public Task RemovePeerAsync(Peer peer) => throw new NotImplementedException();

        public void Connect() => Console.WriteLine("Connect");

        public void Disconnect() => Console.WriteLine("Disconnect");
        public Task<PermissionInfo?> TryReadPermissionInfo(string sourceAddress, string senderId, Network network) => throw new NotImplementedException();

        public Task RespondAsync(BaseBeaconMessage baseBeaconMessage)
        {
            Console.WriteLine("Respond");

            return Task.CompletedTask;
        }
    }
}