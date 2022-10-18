namespace Beacon.Sdk.BeaconClients
{
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using Abstract;
    using Beacon;
    using Beacon.Permission;
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
        private readonly PermissionHandler _permissionHandler;
        private readonly RequestMessageHandler _requestMessageHandler;
        private readonly ResponseMessageHandler _responseMessageHandler;
        private readonly IJsonSerializerService _jsonSerializerService;

        public event EventHandler<BeaconMessageEventArgs> OnBeaconMessageReceived;

        public event EventHandler<DappConnectedEventArgs?> OnDappsListChanged;

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
            RequestMessageHandler requestMessageHandler,
            ResponseMessageHandler responseMessageHandler,
            PermissionHandler permissionHandler,
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
            _requestMessageHandler = requestMessageHandler;
            _responseMessageHandler = responseMessageHandler;
            _permissionHandler = permissionHandler;
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
            _responseMessageHandler.OnDappConnected += _responseMessageHandler_OnDappConnected;
            _p2PCommunicationService.Start();
            Connected = _p2PCommunicationService.Syncing;

            _logger.LogInformation("Dapp client connected {Connected}", Connected);
        }

        public Task<string> GetPairingRequestInfo()
        {
            var pairingRequestInfo = _p2PCommunicationService
                .GetPairingRequestInfo(AppName, KnownRelayServers, IconUrl, AppUrl).Result;
            var pairingString = _jsonSerializerService.Serialize(pairingRequestInfo);
            byte[] pairingBytes = Encoding.UTF8.GetBytes(pairingString);
            string? pairingQrCode = Base58.Convert(pairingBytes);
            return Task.FromResult(pairingQrCode);
        }

        public Task<Peer?> GetActivePeer()
        {
            return _peerRepository.TryGetActive();
        }

        private void _responseMessageHandler_OnDappConnected(object sender, DappConnectedEventArgs e)
        {
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

            foreach (string message in e.Messages)
                await HandleReceivedMessage(message);
        }

        private async Task HandleReceivedMessage(string message)
        {
            (_, BaseBeaconMessage requestMessage) =
                _requestMessageHandler.Handle(message, SenderId);
            
            if (requestMessage.Type == BeaconMessageType.permission_response)
                await _responseMessageHandler.Handle(requestMessage, requestMessage.SenderId);
            
            OnBeaconMessageReceived?.Invoke(this, new BeaconMessageEventArgs(requestMessage.SenderId, requestMessage));
        }

        public async Task SendResponseAsync(string receiverId, BaseBeaconMessage response)
        {
            var peer = _peerRepository.TryReadAsync(receiverId).Result
                       ?? throw new NullReferenceException(nameof(Peer));

            var message = await _responseMessageHandler.Handle(response, receiverId);
            await _p2PCommunicationService.SendMessageAsync(peer, message);
        }
    }
}