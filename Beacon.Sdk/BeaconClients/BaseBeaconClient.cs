namespace Beacon.Sdk.BeaconClients
{
    using System;
    using System.Threading.Tasks;
    using Abstract;
    using Beacon;
    using Core.Domain;
    using Core.Domain.Entities;
    using Core.Domain.Interfaces;
    using Core.Domain.Interfaces.Data;
    using Core.Domain.P2P;
    using Core.Domain.Services;
    using Netezos.Encoding;
    using Utils;

    public abstract class BaseBeaconClient : IBaseBeaconClient
    {
        protected readonly string[] KnownRelayServers;
        protected readonly IP2PCommunicationService P2PCommunicationService;
        protected readonly IPeerRepository PeerRepository;
        protected readonly IJsonSerializerService JsonSerializerService;
        protected readonly SerializeMessageHandler SerializeMessageHandler;
        protected readonly DeserializeMessageHandler DeserializeMessageHandler;
        protected readonly string? AppUrl;
        protected readonly string? IconUrl;
        private readonly KeyPairService _keyPairService;
        public event EventHandler<BeaconMessageEventArgs> OnBeaconMessageReceived;
        public event EventHandler<ConnectedClientsListChangedEventArgs?> OnConnectedClientsListChanged;
        public event Action OnDisconnected;

        protected void RaiseOnBeaconMessageReceived(BeaconMessageEventArgs e)
        {
            OnBeaconMessageReceived?.Invoke(this, e);
        }

        protected BaseBeaconClient(
            KeyPairService keyPairService,
            AccountService accountService,
            IAppMetadataRepository appMetadataRepository,
            IPermissionInfoRepository permissionInfoRepository,
            ISeedRepository seedRepository,
            IP2PCommunicationService p2PCommunicationService,
            IPeerRepository peerRepository,
            SerializeMessageHandler serializeMessageHandler,
            DeserializeMessageHandler deserializeMessageHandler,
            IJsonSerializerService jsonSerializerService,
            BeaconOptions options)
        {
            _keyPairService = keyPairService;
            AccountService = accountService;
            AppMetadataRepository = appMetadataRepository;
            PermissionInfoRepository = permissionInfoRepository;
            SeedRepository = seedRepository;

            IconUrl = options.IconUrl;
            AppUrl = options.AppUrl;

            AppName = options.AppName;
            KnownRelayServers = options.KnownRelayServers;

            P2PCommunicationService = p2PCommunicationService;
            PeerRepository = peerRepository;
            SerializeMessageHandler = serializeMessageHandler;
            DeserializeMessageHandler = deserializeMessageHandler;
            JsonSerializerService = jsonSerializerService;
        }

        public string AppName { get; }
        public bool LoggedIn { get; private set; }
        public bool Connected { get; private set; }

        private HexString BeaconId
        {
            get
            {
                if (!HexString.TryParse(_keyPairService.KeyPair.PublicKey, out HexString beaconId))
                    throw new InvalidOperationException("Can not parse publicKey");

                return beaconId;
            }
        }

        public async Task SendResponseAsync(string receiverId, BaseBeaconMessage response)
        {
            var peer = PeerRepository.TryReadAsync(receiverId).Result
                   ?? throw new NullReferenceException(nameof(Peer) + "is null");

            var message = await SerializeMessageHandler.Handle(response, receiverId);
            await P2PCommunicationService.SendMessageAsync(peer, message);
        }

        public async Task RemovePeerAsync(string peerSenderId)
        {
            var peer = PeerRepository.TryReadAsync(peerSenderId).Result
                       ?? throw new NullReferenceException(nameof(Peer));
            
            await SendResponseAsync(peer.SenderId, new DisconnectMessage(KeyPairService.CreateGuid(), SenderId));
            await P2PCommunicationService.DeleteAsync(peer);
            await PeerRepository.Delete(peer);
            await PermissionInfoRepository.DeleteBySenderIdAsync(peer.SenderId);
            await AppMetadataRepository.Delete(peer.SenderId);
            
            OnConnectedClientsListChanged?.Invoke(this, null);
        }

        public async Task InitAsync()
        {
            await P2PCommunicationService.LoginAsync(KnownRelayServers);
            LoggedIn = P2PCommunicationService.LoggedIn;
        }

        public void Connect()
        {
            P2PCommunicationService.OnP2PMessagesReceived += OnP2PMessagesReceived;
            SerializeMessageHandler.OnPermissionsCreated += ClientPermissionsCreatedHandler;
            P2PCommunicationService.Start();
            Connected = P2PCommunicationService.Syncing;
        }

        public void Disconnect()
        {
            P2PCommunicationService.Stop();
            P2PCommunicationService.OnP2PMessagesReceived -= OnP2PMessagesReceived;
            SerializeMessageHandler.OnPermissionsCreated -= ClientPermissionsCreatedHandler;
            Connected = P2PCommunicationService.Syncing;

            OnDisconnected?.Invoke();
        }

        private void ClientPermissionsCreatedHandler(object sender, ConnectedClientsListChangedEventArgs e)
        {
            OnConnectedClientsListChanged?.Invoke(this, e);
        }

        protected abstract Task OnP2PMessagesReceived(object? sender, P2PMessageEventArgs e);
        protected AccountService AccountService { get; }
        public string SenderId => Base58.Convert(PeerFactory.Hash(BeaconId.ToByteArray(), 5));
        public IAppMetadataRepository AppMetadataRepository { get; }
        public IPermissionInfoRepository PermissionInfoRepository { get; }
        public ISeedRepository SeedRepository { get; }

        public AppMetadata Metadata => new()
        {
            SenderId = SenderId,
            Name = AppName,
            Icon = IconUrl,
            AppUrl = AppUrl
        };
    }
}