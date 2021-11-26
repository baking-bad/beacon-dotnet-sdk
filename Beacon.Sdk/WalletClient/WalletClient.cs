namespace Beacon.Sdk.WalletClient
{
    using System;
    using System.Threading.Tasks;
    using Beacon;
    using Core.Domain;
    using Core.Domain.Interfaces;
    using Core.Domain.Interfaces.Data;
    using Core.Domain.P2P;
    using Core.Domain.Services;
    using Microsoft.Extensions.Logging;
    using Utils;

    /*
     * Todo: add PermissionRepository
     */

    public class WalletClient : BaseClient, IWalletClient
    {
        private readonly IJsonSerializerService _jsonSerializerService;
        private readonly ILogger<WalletClient> _logger;
        private readonly IP2PCommunicationService _p2PCommunicationService;
        private readonly PeerFactory _peerFactory;
        private readonly IPeerRepository _peerRepository;

        public WalletClient(
            ILogger<WalletClient> logger,
            IPeerRepository peerRepository,
            IAppMetadataRepository appMetadataRepository,
            IP2PCommunicationService p2PCommunicationService,
            IJsonSerializerService jsonSerializerService,
            KeyPairService keyPairService,
            PeerFactory peerFactory,
            ClientOptions options) : base(keyPairService, appMetadataRepository, options)
        {
            _logger = logger;
            _peerRepository = peerRepository;
            _p2PCommunicationService = p2PCommunicationService;
            _jsonSerializerService = jsonSerializerService;
            _peerFactory = peerFactory;
        }

        public event EventHandler<BeaconMessageEventArgs>? OnBeaconMessageReceived;

        public async Task InitAsync() => await _p2PCommunicationService.LoginAsync();

        public async Task AddPeerAsync(P2PPairingRequest pairingRequest, bool sendPairingResponse = true)
        {
            if (!HexString.TryParse(pairingRequest.PublicKey, out HexString receiverHexPublicKey))
            {
                _logger.LogError("Can not parse receiver public key");
                return;
            }

            Peer peer = _peerFactory.Create(
                receiverHexPublicKey,
                pairingRequest.Name,
                pairingRequest.Version,
                pairingRequest.RelayServer
            );

            peer = _peerRepository.Create(peer).Result;

            if (sendPairingResponse)
                _ = await _p2PCommunicationService.SendChannelOpeningMessageAsync(peer, pairingRequest.Id, AppName);
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

        private async Task SendAcknowledgeResponseAsync(BeaconBaseMessage beaconBaseMessage)
        {
            var acknowledgeResponse =
                new AcknowledgeResponse(Constants.BeaconVersion, beaconBaseMessage.Id, SenderId);

            Peer peer = _peerRepository.TryRead(beaconBaseMessage.SenderId).Result
                        ?? throw new NullReferenceException(nameof(Peer));

            string message = _jsonSerializerService.Serialize(acknowledgeResponse);

            await _p2PCommunicationService.SendMessageAsync(peer, message);
        }

        private async Task OnP2PMessagesReceived(object? sender, P2PMessageEventArgs e)
        {
            if (sender is not IP2PCommunicationService)
                throw new ArgumentException("sender is not IP2PCommunicationClient");

            foreach (string message in e.Messages)
            {
                BeaconBaseMessage beaconBaseMessage = _jsonSerializerService.Deserialize<BeaconBaseMessage>(message);

                if (beaconBaseMessage.Version != "1")
                    await SendAcknowledgeResponseAsync(beaconBaseMessage);

                OnBeaconMessageReceived?.Invoke(this, new BeaconMessageEventArgs(beaconBaseMessage));
            }
        }
    }
}