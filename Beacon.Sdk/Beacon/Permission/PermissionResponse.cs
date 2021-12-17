namespace Beacon.Sdk.Beacon.Permission
{
    using System.Collections.Generic;
    using Core.Domain;

    public class PermissionResponse : BaseBeaconMessage, IBeaconResponse
    {
        public PermissionResponse(
            string id,
            Network network,
            List<PermissionScope> scopes,
            string publicKey)
            : base(BeaconMessageType.permission_response, id)
        {
            Network = network;
            Scopes = scopes;
            PublicKey = publicKey;
        }

        /// <summary>
        ///     Some additional information about the Wallet
        /// </summary>
        public AppMetadata AppMetadata { get; set; }

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
    }
}