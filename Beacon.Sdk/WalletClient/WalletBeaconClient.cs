namespace Beacon.Sdk.WalletClient
{
    using System;
    using System.Threading.Tasks;
    using Beacon;
    using Core.Domain;
    using Core.Domain.Entities;
    using Core.Domain.Interfaces;
    using Core.Domain.Interfaces.Data;
    using Core.Domain.P2P;
    using Core.Domain.Services;
    using Microsoft.Extensions.Logging;
    using Utils;

    /*
     * Todo: add PermissionRepository
     */

    public class WalletBeaconClient : BaseBeaconClient, IWalletClient
    {
        private readonly ILogger<WalletBeaconClient> _logger;
        private readonly IP2PCommunicationService _p2PCommunicationService;
        private readonly PeerFactory _peerFactory;
        private readonly IPeerRepository _peerRepository;

        private readonly RequestMessageHandler _requestMessageHandler;
        private readonly ResponseMessageHandler _responseMessageHandler;

        public WalletBeaconClient(
            ILogger<WalletBeaconClient> logger,
            IPeerRepository peerRepository,
            IAppMetadataRepository appMetadataRepository,
            IP2PCommunicationService p2PCommunicationService,
            KeyPairService keyPairService,
            PeerFactory peerFactory,
            RequestMessageHandler requestMessageHandler,
            ResponseMessageHandler responseMessageHandler,
            BeaconOptions options)
            : base(keyPairService, appMetadataRepository, options)
        {
            _logger = logger;
            _peerRepository = peerRepository;
            _p2PCommunicationService = p2PCommunicationService;
            _requestMessageHandler = requestMessageHandler;
            _responseMessageHandler = responseMessageHandler;
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

        public async Task SendResponseAsync(string receiverId, IBeaconResponse beaconResponse)
        {
            Peer peer = _peerRepository.TryRead(receiverId).Result
                        ?? throw new NullReferenceException(nameof(Peer));

            string message = _responseMessageHandler.Handle(beaconResponse, Metadata, SenderId, receiverId);

            await _p2PCommunicationService.SendMessageAsync(peer, message);
        }

        private async Task OnP2PMessagesReceived(object? sender, P2PMessageEventArgs e)
        {
            if (sender is not IP2PCommunicationService)
                throw new ArgumentException("sender is not IP2PCommunicationClient");

            foreach (string message in e.Messages)
            {
                (AcknowledgeBeaconResponse ack, IBeaconRequest requestMessage) =
                    _requestMessageHandler.Handle(message, SenderId);

                if (requestMessage.Version != "1")
                    await SendResponseAsync(requestMessage.SenderId, ack);

                OnBeaconMessageReceived?.Invoke(this,
                    new BeaconMessageEventArgs(requestMessage.SenderId, requestMessage));
            }
        }
    }
}