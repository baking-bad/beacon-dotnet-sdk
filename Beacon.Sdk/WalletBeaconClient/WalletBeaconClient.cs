namespace Beacon.Sdk.WalletBeaconClient
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

    public class WalletBeaconClient : BaseBeaconClient, IWalletBeaconClient
    {
        private readonly ILogger<WalletBeaconClient> _logger;
        private readonly IP2PCommunicationService _p2PCommunicationService;
        private readonly PeerFactory _peerFactory;
        private readonly IPeerRepository _peerRepository;
        private readonly PermissionHandler _permissionHandler;

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
            PermissionHandler permissionHandler,
            BeaconOptions options)
            : base(keyPairService, appMetadataRepository, options)
        {
            _logger = logger;
            _peerRepository = peerRepository;
            _p2PCommunicationService = p2PCommunicationService;
            _requestMessageHandler = requestMessageHandler;
            _responseMessageHandler = responseMessageHandler;
            _permissionHandler = permissionHandler;
            _peerFactory = peerFactory;
        }

        public bool LoggedIn { get; private set; }

        public bool Connected { get; private set; }

        public event EventHandler<BeaconMessageEventArgs>? OnBeaconMessageReceived;

        public async Task InitAsync()
        {
            await _p2PCommunicationService.LoginAsync(KnownRelayServers);

            LoggedIn = _p2PCommunicationService.LoggedIn;
        }

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

            Connected = _p2PCommunicationService.Syncing;
        }

        public void Disconnect()
        {
            _p2PCommunicationService.Stop();
            _p2PCommunicationService.OnP2PMessagesReceived -= OnP2PMessagesReceived;

            Connected = _p2PCommunicationService.Syncing;
        }

        public async Task SendResponseAsync(string receiverId, IBeaconResponse beaconResponse)
        {
            Peer peer = _peerRepository.TryRead(receiverId).Result
                        ?? throw new NullReferenceException(nameof(Peer));

            string message = _responseMessageHandler.Handle(beaconResponse, receiverId);

            await _p2PCommunicationService.SendMessageAsync(peer, message);
        }

        private async Task OnP2PMessagesReceived(object? sender, P2PMessageEventArgs e)
        {
            if (sender is not IP2PCommunicationService)
                throw new ArgumentException("sender is not IP2PCommunicationClient");

            foreach (string message in e.Messages)
                await HandleMessage(message);
        }

        private async Task HandleMessage(string message)
        {
            (AcknowledgeResponse ack, IBeaconRequest requestMessage) =
                _requestMessageHandler.Handle(message, SenderId);

            if (requestMessage.Version != "1")
                await SendResponseAsync(requestMessage.SenderId, ack);

            bool hasPermission = await _permissionHandler.HasPermission(requestMessage);

            if (hasPermission)
                OnBeaconMessageReceived?.Invoke(this,
                    new BeaconMessageEventArgs(requestMessage.SenderId, requestMessage));
            else
                _logger.LogInformation("Received message have not permission");
        }
    }
}