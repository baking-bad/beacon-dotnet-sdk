namespace Beacon.Sdk
{
    using System;
    using System.Threading.Tasks;
    using Core.Beacon;
    using Core.Domain.P2P.Dto.Handshake;
    using Core.Utils;

    public interface IWalletBeaconClient
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