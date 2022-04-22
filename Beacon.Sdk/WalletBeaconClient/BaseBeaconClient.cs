namespace Beacon.Sdk.WalletBeaconClient
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
        private readonly string? _appUrl;
        private readonly string? _iconUrl;
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

            _iconUrl = options.IconUrl;
            _appUrl = options.AppUrl;

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
            Icon = _iconUrl
        };
    }
}