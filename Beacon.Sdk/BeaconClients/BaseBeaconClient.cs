namespace Beacon.Sdk.BeaconClients
{
    using System;
    using Beacon;
    using Core.Domain.Entities;
    using Core.Domain.Interfaces.Data;
    using Core.Domain.Services;
    using Netezos.Encoding;
    using Utils;

    public abstract class BaseBeaconClient
    {
        protected readonly string? AppUrl;
        protected readonly string? IconUrl;
        private readonly KeyPairService _keyPairService;

        protected readonly string AppName;
        protected readonly string[] KnownRelayServers;

        protected BaseBeaconClient(
            KeyPairService keyPairService,
            AccountService accountService,
            IAppMetadataRepository appMetadataRepository,
            IPermissionInfoRepository permissionInfoRepository,
            ISeedRepository seedRepository,
            BeaconOptions options)
        {
            _keyPairService = keyPairService;
            AccountService = accountService;
            AppMetadataRepository = appMetadataRepository;
            PermissionInfoRepository = permissionInfoRepository;
            SeedRepository = seedRepository;

            IconUrl = options.IconUrl;
            AppUrl = options.AppUrl;

            AppName = options.AppName;
            KnownRelayServers = options.KnownRelayServers;
        }

        private HexString BeaconId
        {
            get
            {
                if (!HexString.TryParse(_keyPairService.KeyPair.PublicKey, out HexString beaconId))
                    throw new InvalidOperationException("Can not parse publicKey");

                return beaconId;
            }
        }

        protected AccountService AccountService { get; }

        public string SenderId => Base58.Convert(PeerFactory.Hash(BeaconId.ToByteArray(), 5));

        public IAppMetadataRepository AppMetadataRepository { get; }

        public IPermissionInfoRepository PermissionInfoRepository { get; }

        public ISeedRepository SeedRepository { get; }

        public AppMetadata Metadata => new()
        {
            SenderId = SenderId,
            Name = AppName,
            Icon = IconUrl
        };
    }
}