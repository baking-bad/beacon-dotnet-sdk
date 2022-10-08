namespace Beacon.Sdk.BeaconClients
{
    using System.Threading.Tasks;
    using Abstract;
    using Core.Domain;
    using Core.Domain.Entities;
    using Core.Domain.Interfaces;
    using Core.Domain.Interfaces.Data;
    using Core.Domain.P2P;
    using Core.Domain.Services;
    using Microsoft.Extensions.Logging;

    public class DappBeaconClient : BaseBeaconClient, IDappBeaconClient
    {
        private readonly ILogger<DappBeaconClient> _logger;
        private readonly IP2PCommunicationService _p2PCommunicationService;
        private readonly PeerFactory _peerFactory;
        private readonly IPeerRepository _peerRepository;
        private readonly PermissionHandler _permissionHandler;

        private readonly RequestMessageHandler _requestMessageHandler;
        private readonly ResponseMessageHandler _responseMessageHandler;

        public DappBeaconClient(
            ILogger<DappBeaconClient> logger,
            IPeerRepository peerRepository,
            IAppMetadataRepository appMetadataRepository,
            IPermissionInfoRepository permissionInfoRepository,
            ISeedRepository seedRepository,
            IP2PCommunicationService p2PCommunicationService,
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

        private void _responseMessageHandler_OnDappConnected(object sender, DappConnectedEventArgs e)
        {
        }

        private Task OnP2PMessagesReceived(object? sender, P2PMessageEventArgs e)
        {
            if (e.Messages.Count > 0)
                _logger.LogInformation("DAPP: OnP2PMessagesReceived {Message}", e.Messages.Count);
            return Task.FromResult(true);
        }
    }
}