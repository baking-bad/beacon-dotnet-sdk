namespace Beacon.Sdk.BeaconClients.Abstract
{
    using System.Threading.Tasks;
    using Beacon;
    using Beacon.Permission;
    using Core.Domain.Entities;

    public interface IWalletBeaconClient : IBaseBeaconClient
    {
        Task AddPeerAsync(P2PPairingRequest pairingRequest, bool sendPairingResponse = true);
        Task<PermissionInfo?> TryReadPermissionInfo(string sourceAddress, string senderId, Network network);
        P2PPairingRequest GetPairingRequest(string pairingData);
    }
}