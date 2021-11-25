namespace Beacon.Sdk
{
    using System;
    using System.Threading.Tasks;
    using Beacon;
    using Utils;

    public class MockWalletClient : IWalletClient
    {
        public event EventHandler<BeaconMessageEventArgs>? OnBeaconMessageReceived;

        public HexString BeaconId { get; }

        public string AppName { get; }

        public Task RespondAsync(BeaconBaseMessage beaconBaseMessage)
        {
            Console.WriteLine("Respond");

            return Task.CompletedTask;
        }

        public Task InitAsync()
        {
            Console.WriteLine("Init");

            return Task.CompletedTask;
        }

        public async Task AddPeerAsync(P2PPairingRequest pairingRequest, bool sendPairingResponse = true)
        {
            await Task.Delay(100);

            var eventArgs = new BeaconMessageEventArgs(new BeaconBaseMessage("type", "2", "id", "senderId"));
            OnBeaconMessageReceived?.Invoke(this, eventArgs);

            await Task.CompletedTask;
        }

        public void Connect() => Console.WriteLine("Connect");

        public void Disconnect() => Console.WriteLine("Disconnect");
    }
}