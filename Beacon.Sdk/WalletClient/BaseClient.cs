namespace Beacon.Sdk.WalletClient
{
    using System;
    using Base58Check;
    using Beacon;
    using Core.Domain;
    using Core.Domain.Services;
    using Utils;

    public abstract class BaseClient
    {
        private readonly string? _appUrl;
        private readonly string? _iconUrl;
        private readonly KeyPairService _keyPairService;
        protected readonly string AppName;

        protected BaseClient(KeyPairService keyPairService, IAppMetadataRepository appMetadataRepository,
            ClientOptions options)
        {
            _keyPairService = keyPairService;
            AppMetadataRepository = appMetadataRepository;
            AppName = options.AppName;
            _iconUrl = options.IconUrl;
            _appUrl = options.AppUrl;
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

        protected string SenderId => Base58CheckEncoding.Encode(PeerFactory.Hash(BeaconId.ToByteArray(), 5));

        public IAppMetadataRepository AppMetadataRepository { get; }

        public AppMetadata Metadata => new()
        {
            SenderId = SenderId,
            Name = AppName,
            Icon = _iconUrl
        };
    }
}