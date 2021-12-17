namespace Beacon.Sdk.Sample.Console
{
    using System;
    using System.Threading.Tasks;
    using Beacon;
    using Core.Domain;
    using Utils;

    public class MockRequest : IBeaconRequest
    {
        public string Id { get; }
        public string SenderId { get; }
        public string Version { get; }
        public BeaconMessageType Type { get; }
    }

    public class MockWalletBeaconClient : IWalletBeaconClient
    {
        public HexString BeaconId { get; }

        public string AppName { get; }

        public bool LoggedIn { get; }

        public bool Connected { get; }

        public AppMetadata Metadata { get; }

        public IAppMetadataRepository AppMetadataRepository { get; }

        public event EventHandler<BeaconMessageEventArgs>? OnBeaconMessageReceived;

        public Task SendResponseAsync(string receiverId, IBeaconResponse response) =>
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
            OnBeaconMessageReceived?.Invoke(this, new BeaconMessageEventArgs("", new MockRequest()));

            await Task.CompletedTask;
        }

        public void Connect() => Console.WriteLine("Connect");

        public void Disconnect() => Console.WriteLine("Disconnect");

        public Task RespondAsync(BaseBeaconMessage baseBeaconMessage)
        {
            Console.WriteLine("Respond");

            return Task.CompletedTask;
        }
    }
}