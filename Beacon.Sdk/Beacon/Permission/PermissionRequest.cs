// ReSharper disable InconsistentNaming

namespace Beacon.Sdk.Beacon.Permission
{
    using System.Collections.Generic;

    public record PermissionRequest : BaseBeaconMessage
    {
        public PermissionRequest(
            BeaconMessageType type,
            string version,
            string id,
            string senderId,
            AppMetadata appMetadata,
            Network network,
            List<PermissionScope> scopes)
            : base(type, version, id, senderId)
        {
            AppMetadata = appMetadata;
            Network = network;
            Scopes = scopes;
        }

        /// <summary>
        ///     Some additional information about the DApp
        /// </summary>
        public AppMetadata AppMetadata { get; }

        /// <summary>
        ///     Network on which the permissions are requested. Only one network can be specified.
        ///     In case you need permissions on multiple networks, you need to request permissions multiple times.
        /// </summary>
        public Network Network { get; }

        /// <summary>
        ///     The permission scopes that the DApp is asking for
        /// </summary>
        public List<PermissionScope> Scopes { get; }
    }
}