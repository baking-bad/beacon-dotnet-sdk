namespace Beacon.Sdk.BeaconClients
{
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using Abstract;
    using Beacon;
    using Core.Domain;
    using Core.Domain.Entities;
    using Core.Domain.Interfaces;
    using Core.Domain.Interfaces.Data;
    using Core.Domain.P2P;
    using Core.Domain.Services;
    using Microsoft.Extensions.Logging;
    using Netezos.Encoding;

    public class DappBeaconClient : BaseBeaconClient, IDappBeaconClient
    {
        private readonly ILogger<DappBeaconClient> _logger;
        private readonly IP2PCommunicationService _p2PCommunicationService;
        private readonly PeerFactory _peerFactory;
        private readonly IPeerRepository _peerRepository;
        private readonly DeserializeMessageHandler _deserializeMessageHandler;
        private readonly SerializeMessageHandler _serializeMessageHandler;
        private readonly IJsonSerializerService _jsonSerializerService;

        public event EventHandler<BeaconMessageEventArgs> OnBeaconMessageReceived;
        public event EventHandler<ConnectedClientsListChangedEventArgs?> OnConnectedClientsListChanged;

        public DappBeaconClient(
            ILogger<DappBeaconClient> logger,
            IPeerRepository peerRepository,
            IAppMetadataRepository appMetadataRepository,
            IPermissionInfoRepository permissionInfoRepository,
            ISeedRepository seedRepository,
            IP2PCommunicationService p2PCommunicationService,
            IJsonSerializerService jsonSerializerService,
            AccountService accountService,
            KeyPairService keyPairService,
            PeerFactory peerFactory,
            DeserializeMessageHandler deserializeMessageHandler,
            SerializeMessageHandler serializeMessageHandler,
            BeaconOptions options)
            : base(keyPairService,
                accountService,
                appMetadataRepository,
                permissionInfoRepository,
                seedRepository,
                options)
        {
            _logger = logger;

            _peerRepository = peerRepository;
            _p2PCommunicationService = p2PCommunicationService;
            _deserializeMessageHandler = deserializeMessageHandler;
            _serializeMessageHandler = serializeMessageHandler;
            _peerFactory = peerFactory;
            _jsonSerializerService = jsonSerializerService;
        }

        public bool LoggedIn { get; private set; }
        public bool Connected { get; private set; }

        public async Task InitAsync()
        {
            await _p2PCommunicationService.LoginAsync(KnownRelayServers);
            LoggedIn = _p2PCommunicationService.LoggedIn;
            _logger.LogInformation("Dapp client Logged In {LoggedIn}", LoggedIn);
        }

        public void Connect()
        {
            _p2PCommunicationService.OnP2PMessagesReceived += OnP2PMessagesReceived;
            _serializeMessageHandler.OnPermissionsCreated += ClientPermissionsCreatedHandler;
            _p2PCommunicationService.Start();
            Connected = _p2PCommunicationService.Syncing;

            _logger.LogInformation("Dapp client connected {Connected}", Connected);
        }

        public string GetPairingRequestInfo()
        {
            var pairingRequestInfo = _p2PCommunicationService
                .GetPairingRequestInfo(AppName, KnownRelayServers, IconUrl, AppUrl).Result;
            var pairingString = _jsonSerializerService.Serialize(pairingRequestInfo);
            var pairingBytes = Encoding.UTF8.GetBytes(pairingString);
            var pairingQrCode = Base58.Convert(pairingBytes);
            return pairingQrCode;
        }

        public Peer? GetActivePeer()
        {
            return _peerRepository.TryGetActive().Result;
        }

        public PermissionInfo? GetActivePeerPermissions()
        {
            var activePeer = GetActivePeer();
            if (activePeer == null) return null;

            return PermissionInfoRepository
                .TryReadBySenderIdAsync(activePeer.SenderId)
                .Result;
        }

        private void ClientPermissionsCreatedHandler(object sender, ConnectedClientsListChangedEventArgs e)
        {
            OnConnectedClientsListChanged?.Invoke(this,
                new ConnectedClientsListChangedEventArgs(e.Metadata, e.PermissionInfo));
        }

        private async Task OnP2PMessagesReceived(object? sender, P2PMessageEventArgs e)
        {
            if (sender is not IP2PCommunicationService)
                throw new ArgumentException("sender is not IP2PCommunicationClient");

            if (e.PairingResponse != null)
            {
                var peer = _peerRepository.TryGetActive().Result;
                if (peer == null) return;

                var appMetaData = new AppMetadata
                {
                    SenderId = peer.SenderId,
                    Name = e.PairingResponse.Name,
                    Icon = e.PairingResponse.Icon,
                    AppUrl = e.PairingResponse.AppUrl
                };
                await AppMetadataRepository.CreateOrUpdateAsync(appMetaData);

                OnBeaconMessageReceived?.Invoke(this, new BeaconMessageEventArgs(null, null, true));
                return;
            }

            foreach (var message in e.Messages)
                await HandleReceivedMessage(message);
        }

        private async Task HandleReceivedMessage(string message)
        {
            var (_, receivedMessage) =
                _deserializeMessageHandler.Handle(message, SenderId);

            if (receivedMessage.Type == BeaconMessageType.permission_response)
                await _serializeMessageHandler.Handle(receivedMessage, receivedMessage.SenderId);

            if (receivedMessage.Type == BeaconMessageType.disconnect)
                await RemovePeerAsync(receivedMessage.SenderId);

            OnBeaconMessageReceived?.Invoke(this,
                new BeaconMessageEventArgs(receivedMessage.SenderId, receivedMessage));
        }

        public async Task SendResponseAsync(string receiverId, BaseBeaconMessage response)
        {
            var peer = _peerRepository.TryReadAsync(receiverId).Result
                       ?? throw new NullReferenceException(nameof(Peer));

            var message = await _serializeMessageHandler.Handle(response, receiverId);
            await _p2PCommunicationService.SendMessageAsync(peer, message);
        }

        private async Task RemovePeerAsync(string peerSenderId)
        {
            var peer = _peerRepository.TryReadAsync(peerSenderId).Result
                       ?? throw new NullReferenceException(nameof(Peer));

            await SendResponseAsync(peer.SenderId, new DisconnectMessage(KeyPairService.CreateGuid(), SenderId));

            await _p2PCommunicationService.DeleteAsync(peer);
            await _peerRepository.Delete(peer);
            await PermissionInfoRepository.DeleteBySenderIdAsync(peer.SenderId);
            await AppMetadataRepository.Delete(peer.SenderId);

            OnConnectedClientsListChanged?.Invoke(this, null);
        }
    }
}