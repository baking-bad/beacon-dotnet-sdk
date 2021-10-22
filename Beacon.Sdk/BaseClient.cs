namespace Beacon.Sdk
{
    using System;
    using Core.Infrastructure.Cryptography;
    using Sodium;

    public class BeaconClientConfiguration
    {
        public string Name { get; init; }

        public string? IconUrl { get; init; }

        public string? AppUrl { get; init; }
    }

    // The beacon client is an abstract client that handles everything that is shared between all other clients.
    // Specifically, it handles managing the beaconId and and the local keypair.
    public abstract class BaseClient
    {
        public readonly string? AppUrl;

        public readonly string? IconUrl;

        public readonly string Name;

        /*
         * The beaconId is a public key that is used to identify one specific application (dapp or wallet).
         * This is used inside a message to specify the sender, for example.
         */
        protected string BeaconId = null!;

        protected KeyPair KeyPair = null!;

        public BaseClient(BeaconClientConfiguration configuration)
        {
            Name = configuration.Name ?? throw new NullReferenceException("Name not set");
            IconUrl = configuration.IconUrl;
            AppUrl = configuration.AppUrl;

            InitializeSdk();
        }

        // This method initializes the SDK by setting some values in the storage and generating a keypair.
        private void InitializeSdk()
        {
            var seed = Guid.NewGuid().ToString();

            KeyPair = BeaconCryptographyService.GenerateEd25519KeyPair(seed);
            BeaconId = BeaconCryptographyService.ToHexString(KeyPair.PublicKey);
        }


        // private static BeaconWalletClient? s_instance;

        // public BeaconWalletClient GetInstance(BeaconClientConfiguration configuration)
        // {
        //     if (s_instance != null)
        //         return s_instance;
        //
        //     s_instance = new BeaconWalletClient(configuration);
        //         
        //     return s_instance;
        //
        // }
    }

    public class BeaconWalletClient : BaseClient
    {
        public BeaconWalletClient(BeaconClientConfiguration configuration) : base(configuration)
        {
            // this.permissionManager = new PermissionManager(new LocalStorage())
            // this.appMetadataManager = new AppMetadataManager(new LocalStorage())
        }
    }
}