namespace Beacon.Sdk.BeaconClients.Abstract
{
    using Core.Domain.Entities;

    public interface IDappBeaconClient : IBaseBeaconClient
    {
        string GetPairingRequestInfo();
        Peer? GetActivePeer();
        PermissionInfo? GetActivePeerPermissions();
    }
}