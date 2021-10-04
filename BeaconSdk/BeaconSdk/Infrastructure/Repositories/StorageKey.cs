// ReSharper disable InconsistentNaming

namespace BeaconSdk.Infrastructure.Repositories
{
    public class StorageKey
    {
        private StorageKey(string value) { Value = value; }

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string Value { get; }

        public static StorageKey TRANSPORT_P2P_PEERS_DAPP => new("beacon:communication-peers-dapp");
        public static StorageKey TRANSPORT_P2P_PEERS_WALLET => new("beacon:communication-peers-wallet");
        public static StorageKey TRANSPORT_POSTMESSAGE_PEERS_DAPP => new("beacon:postmessage-peers-dapp");
        public static StorageKey TRANSPORT_POSTMESSAGE_PEERS_WALLET => new("beacon:postmessage-peers-wallet");
        public static StorageKey ACCOUNTS => new("beacon:accounts");
        public static StorageKey ACTIVE_ACCOUNT => new("beacon:active-account");
        public static StorageKey BEACON_SDK_SECRET_SEED => new("beacon:sdk-secret-seed");
        public static StorageKey APP_METADATA_LIST => new("beacon:app-metadata-list");
        public static StorageKey PERMISSION_LIST => new("beacon:permissions");
        public static StorageKey BEACON_SDK_VERSION => new("beacon:sdk_version");
        public static StorageKey MATRIX_PRESERVED_STATE => new("beacon:sdk-matrix-preserved-state");
        public static StorageKey MATRIX_PEER_ROOM_IDS => new("beacon:matrix-peer-rooms");
        public static StorageKey MATRIX_SELECTED_NODE => new("beacon:matrix-selected-node");
        public static StorageKey MULTI_NODE_SETUP_DONE => new("beacon:multi-node-setup");
    }
}