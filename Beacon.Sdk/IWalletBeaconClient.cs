namespace Beacon.Sdk
{
    using System;
    using System.Threading.Tasks;
    using Core.Transport.P2P.Dto.Handshake;

    public class BeaconMessageEventArgs : EventArgs
    {
        
    }
    
    public interface IWalletBeaconClient
    {
        event EventHandler<BeaconMessageEventArgs> OnBeaconMessageReceived;
        
        string AppName { get; }

        Task InitAsync();

        Task AddPeerAsync(P2PPairingRequest pairingRequest, bool sendPairingResponse = true);

        void Connect();

        void Disconnect();
    }
}