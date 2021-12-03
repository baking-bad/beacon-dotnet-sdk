namespace Beacon.Sdk.WalletClient
{
    using System;
    using Beacon;
    using Core.Domain.Entities;
    using Core.Domain.Services;
    using Netezos.Encoding;
    using Utils;

    public abstract class BaseBeaconClient
    {
        private readonly string? _appUrl;
        private readonly string? _iconUrl;
        private readonly KeyPairService _keyPairService;
        protected readonly string AppName;

        protected BaseBeaconClient(KeyPairService keyPairService, IAppMetadataRepository appMetadataRepository,
            BeaconOptions options)
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

        protected string SenderId => Base58.Convert(PeerFactory.Hash(BeaconId.ToByteArray(), 5));

        public IAppMetadataRepository AppMetadataRepository { get; }

        public AppMetadata Metadata => new()
        {
            SenderId = SenderId,
            Name = AppName,
            Icon = _iconUrl
        };
    }
}