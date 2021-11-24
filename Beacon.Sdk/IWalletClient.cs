namespace Beacon.Sdk
{
    using System;
    using System.Threading.Tasks;
    using Beacon;
    using Utils;

    public interface IWalletClient
    {
        HexString BeaconId { get; }

        string AppName { get; }
        
        event EventHandler<BeaconMessageEventArgs> OnBeaconMessageReceived;

        Task RespondAsync(BeaconBaseMessage beaconBaseMessage);

        Task InitAsync();

        Task AddPeerAsync(P2PPairingRequest pairingRequest, bool sendPairingResponse = true);

        void Connect();

        void Disconnect();
    }
}