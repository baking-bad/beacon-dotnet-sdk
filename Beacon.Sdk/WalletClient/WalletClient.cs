namespace Beacon.Sdk.WalletClient
{
    using System;
    using System.Threading.Tasks;
    using Beacon;
    using Core.Domain;
    using Core.Domain.Interfaces;
    using Core.Domain.Interfaces.Data;
    using Core.Domain.Services;
    using Utils;
    using Microsoft.Extensions.Logging;

    /*
     * Todo: add AppMetadataRepository
     */
    public partial class WalletClient : IWalletClient
    {
        private readonly IPeerRepository _peerRepository;
        private readonly ICryptographyService _cryptographyService;
        private readonly IJsonSerializerService _jsonSerializerService;

        private readonly KeyPairService _keyPairService;
        private readonly ILogger<WalletClient> _logger;
        private readonly IP2PCommunicationService _p2PCommunicationService;
        private readonly IPeerRoomRepository _peerRoomRepository;

        public WalletClient(
            ILogger<WalletClient> logger,
            IPeerRepository peerRepository,
            IPeerRoomRepository peerRoomRepository,
            ICryptographyService cryptographyService,
            IP2PCommunicationService p2PCommunicationService,
            IJsonSerializerService jsonSerializerService,
            KeyPairService keyPairService,
            WalletClientOptions options)
        {
            _logger = logger;
            _peerRepository = peerRepository;
            _peerRoomRepository = peerRoomRepository;
            _cryptographyService = cryptographyService;
            _p2PCommunicationService = p2PCommunicationService;
            _jsonSerializerService = jsonSerializerService;
            _keyPairService = keyPairService;
            AppName = options.AppName;
        }

        public event EventHandler<BeaconMessageEventArgs>? OnBeaconMessageReceived;

        public HexString BeaconId
        {
            get
            {
                if (!HexString.TryParse(_keyPairService.KeyPair.PublicKey, out HexString beaconId))
                    throw new InvalidOperationException("Can not parse publicKey");

                return beaconId;
            }
        }

        public string AppName { get; }

        public async Task InitAsync() => await _p2PCommunicationService.LoginAsync();

        public async Task AddPeerAsync(P2PPairingRequest pairingRequest, bool sendPairingResponse = true)
        {
            if (!HexString.TryParse(pairingRequest.PublicKey, out HexString receiverHexPublicKey))
            {
                _logger.LogError("Can not parse receiver public key");
                return;
            }

            Peer peer = Peer.Factory.Create(
                _cryptographyService,
                pairingRequest.Name,
                pairingRequest.RelayServer,
                receiverHexPublicKey,
                pairingRequest.Version);

            peer = _peerRepository.Create(peer).Result;

            if (sendPairingResponse)
            {
                PeerRoom peerRoom =
                    await _p2PCommunicationService.SendChannelOpeningMessageAsync(peer, pairingRequest.Id,
                        AppName);

                _ = _peerRoomRepository.CreateOrUpdate(peerRoom).Result;
            }
        }

        public async Task RespondAsync(BeaconBaseMessage beaconBaseMessage)
        {
            string message = _jsonSerializerService.Serialize(beaconBaseMessage);

            // BeaconPeer beaconPeer = _beaconPeerRepository.TryReadByUserId();
        }

        public void Connect()
        {
            _p2PCommunicationService.OnP2PMessagesReceived += OnP2PMessagesReceived;
            _p2PCommunicationService.Start();
        }

        public void Disconnect()
        {
            _p2PCommunicationService.Stop();
            _p2PCommunicationService.OnP2PMessagesReceived -= OnP2PMessagesReceived;
        }
    }
}