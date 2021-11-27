// ReSharper disable InconsistentNaming
namespace Beacon.Sdk.Beacon.Permission
{
    using System.Collections.Generic;

    public record PermissionRequest(
            BeaconMessageType Type,
            string Version,
            string Id,
            string SenderId,
            AppMetadata AppMetadata,
            Network Network,
            List<PermissionScope> Scopes)
        : BeaconBaseMessage(
            Type,
            Version,
            Id,
            SenderId)
    {
        /// <summary>
        /// Some additional information about the DApp
        /// </summary>
        public AppMetadata AppMetadata { get; } = AppMetadata;
    
        /// <summary>
        /// Network on which the permissions are requested. Only one network can be specified.
        /// In case you need permissions on multiple networks, you need to request permissions multiple times.
        /// </summary>
        public Network Network { get; } = Network;
    
        /// <summary>
        /// The permission scopes that the DApp is asking for
        /// </summary>
        public List<PermissionScope> Scopes { get; } = Scopes;
    }
    
    // public class PermissionRequest : BeaconBaseMessage
    // {
    //     [JsonConstructor]
    //     public PermissionRequest(BeaconMessageType type, string version, string id, string senderId) 
    //         : base(type, version, id, senderId)
    //     {
    //     }
    //     
    //     /// <summary>
    //     /// Some additional information about the DApp
    //     /// </summary>
    //     public AppMetadata AppMetadata { get; }
    //
    //     /// <summary>
    //     /// Network on which the permissions are requested. Only one network can be specified.
    //     /// In case you need permissions on multiple networks, you need to request permissions multiple times.
    //     /// </summary>
    //     public Network Network { get; }
    //
    //     /// <summary>
    //     /// The permission scopes that the DApp is asking for
    //     /// </summary>
    //     public List<PermissionScope> Scopes { get; }
    // }
}