namespace Beacon.Sdk.Beacon.Permission
{
    using System.Collections.Generic;

    public record PermissionResponse(
            string Version,
            string Id,
            string SenderId,
            AppMetadata AppMetadata,
            Network Network,
            List<PermissionScope> Scopes,
            string PublicKey
        )
        : BeaconBaseMessage(
            BeaconMessageType.permission_response,
            Version,
            Id,
            SenderId)
    {
        /// <summary>
        /// Some additional information about the Wallet
        /// </summary>
        public AppMetadata AppMetadata { get; set; } = AppMetadata;

        /// <summary>
        /// Network on which the permissions have been granted
        /// </summary>
        public Network Network { get; } = Network;

        /// <summary>
        /// Permissions that have been granted for this specific address / account
        /// </summary>
        public List<PermissionScope> Scopes { get; } = Scopes;

        /// <summary>
        /// Public Key, because it can be used to verify signatures
        /// </summary>
        public string PublicKey { get; } = PublicKey;
    }
}