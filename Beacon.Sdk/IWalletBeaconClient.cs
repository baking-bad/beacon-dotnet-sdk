namespace Beacon.Sdk
{
    using System;
    using System.Threading.Tasks;
    using Core.Beacon;
    using Core.Transport.P2P.Dto.Handshake;
    using Core.Utils;

    public interface IWalletBeaconClient
    {
        event EventHandler<BeaconMessageEventArgs> OnBeaconMessageReceived;
        
        HexString BeaconId { get; }
        
        string AppName { get; }

        Task RespondAsync(BeaconBaseMessage beaconBaseMessage);

        Task InitAsync();

        Task AddPeerAsync(P2PPairingRequest pairingRequest, bool sendPairingResponse = true);

        void Connect();

        void Disconnect();
    }
}