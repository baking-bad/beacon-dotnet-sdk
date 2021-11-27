namespace Beacon.Sdk.Beacon.Permission
{
    using System.Collections.Generic;

    public record PermissionResponse(
            string Id, 
            string SenderId, 
            AppMetadata AppMetadata,
            Network Network, 
            List<PermissionScope> Scopes,
            string PublicKey)
        : BeaconBaseMessage(BeaconMessageType.permission_response, Constants.MessageVersion, Id, SenderId)
    {

        public PermissionResponse(string id, Network network, List<PermissionScope> scopes, string publicKey) 
            : this(id, string.Empty, new AppMetadata(), network, scopes, publicKey)
        {
            
        }
        
        /// <summary>
        /// Some additional information about the Wallet
        /// </summary>
        public AppMetadata AppMetadata { get; } = AppMetadata;

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

// public PermissionResponse(
//     string id, 
//     string senderId, 
//     AppMetadata appMetadata, 
//     Network network, 
//     List<PermissionScope> scopes, 
//     string publicKey) 
//     : base(BeaconMessageType.permission_response, id, senderId)
// {
//     AppMetadata = appMetadata;
//     Network = network;
//     Scopes = scopes;
//     PublicKey = publicKey;
// }
//
// public PermissionResponse(
//     string id, 
//     Network network, 
//     List<PermissionScope> scopes, 
//     string publicKey) 
//     : this(BeaconMessageType.permission_response, id, string.Empty)
// {
//     AppMetadata = new AppMetadata();
//     Network = network;
//     Scopes = scopes;
//     PublicKey = publicKey;
// }

// string Version,
// string Id,
// string SenderId,
//     AppMetadata AppMetadata,
// Network Network,
//     List<PermissionScope> Scopes,
// string PublicKey
//     )

// public PermissionResponse(string id, Network network, List<PermissionScope> scopes, string publicKey) 
        //     : this(id, network, scopes, publicKey)
        // {
        //     Network = network;
        //     Scopes = scopes;
        //     PublicKey = publicKey;
        //     AppMetadata = new AppMetadata();
        // }