namespace Beacon.Sdk.BeaconClients.Abstract
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Beacon.Operation;
    using Beacon.Permission;
    using Beacon.Sign;
    using Core.Domain.Entities;

    public interface IDappBeaconClient : IBaseBeaconClient
    {
        string GetPairingRequestInfo();
        Peer? GetActivePeer();
        PermissionInfo? GetActiveAccount();
        Task RequestPermissions(IEnumerable<PermissionScope> permissions, Network network);
        Task RequestOperation(IEnumerable<TezosBaseOperation> operations);
        Task RequestSign(string payload, SignPayloadType payloadType);
    }
}