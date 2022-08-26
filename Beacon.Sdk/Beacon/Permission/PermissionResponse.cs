namespace Beacon.Sdk.Beacon.Permission
{
    using System.Collections.Generic;

    public record PermissionResponse : BaseBeaconMessage
    {
        public PermissionResponse(
            string id,
            string senderId,
            AppMetadata appMetadata,
            Network network,
            List<PermissionScope> scopes,
            string publicKey,
            string address,
            string version)
            : base(BeaconMessageType.permission_response, version, id, senderId)
            {
            AppMetadata = appMetadata;
            Network = network;
            Scopes = scopes;
            PublicKey = publicKey;
            Address = address;
        }

        /// <summary>
        ///     Some additional information about the Wallet
        /// </summary>
        public AppMetadata AppMetadata { get; }

        /// <summary>
        ///     Network on which the permissions have been granted
        /// </summary>
        public Network Network { get; }

        /// <summary>
        ///     Permissions that have been granted for this specific address / account
        /// </summary>
        public List<PermissionScope> Scopes { get; }

        /// <summary>
        ///     Public Key, because it can be used to verify signatures
        /// </summary>
        public string PublicKey { get; }
        
        public string Address { get; }
    }
}