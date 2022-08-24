using System.Collections.Generic;
using Beacon.Sdk.Beacon.Sign;

namespace Beacon.Sdk.WalletBeaconClient
{
    using System;
    using System.Threading.Tasks;
    using Beacon;
    using Beacon.Operation;
    using Beacon.Permission;
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
            : base(keyPairService, accountService, appMetadataRepository, permissionInfoRepository, seedRepository,
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

        public event EventHandler<BeaconMessageEventArgs> OnBeaconMessageReceived;

        public event EventHandler<DappConnectedEventArgs?> OnDappsListChanged;

        public async Task InitAsync()
        {
            await _p2PCommunicationService.LoginAsync(KnownRelayServers);

            LoggedIn = _p2PCommunicationService.LoggedIn;
        }

        public async Task AddPeerAsync(P2PPairingRequest pairingRequest, string addressToConnect,
            bool sendPairingResponse = true)
        {
            if (!HexString.TryParse(pairingRequest.PublicKey, out var peerHexPublicKey))
            {
                _logger.LogError("Can not parse receiver public key");
                return;
            }

            var peer = _peerFactory.Create(
                peerHexPublicKey,
                pairingRequest.Name,
                pairingRequest.Version,
                pairingRequest.RelayServer,
                addressToConnect
            );

            peer = _peerRepository.CreateAsync(peer).Result;

            if (sendPairingResponse)
                _ = await _p2PCommunicationService.SendChannelOpeningMessageAsync(peer, pairingRequest.Id, AppName);
        }

        public IEnumerable<Peer> GetAllPeers()
        {
            return _peerRepository.GetAll().Result;
        }

        public async Task RemovePeerAsync(string peerSenderId)
        {
            var peer = _peerRepository.TryReadAsync(peerSenderId).Result
                       ?? throw new NullReferenceException(nameof(Peer));

            try
            {
                await SendDisconnectMessage(peer.SenderId);
            }
            catch (Exception)
            {
                _logger.LogError("{@Sender} Error during sending removing peer {@PeerId} request",
                    "Beacon", peerSenderId);
            }

            await _p2PCommunicationService.DeleteAsync(peer);
            await _peerRepository.Delete(peer);
            await PermissionInfoRepository.DeleteBySenderIdAsync(peer.SenderId);
            await AppMetadataRepository.Delete(peer.SenderId);

            OnDappsListChanged?.Invoke(this, null);
        }

        private Task SendDisconnectMessage(string receiverId)
        {
            var response = new DisconnectMessage(KeyPairService.CreateGuid(), SenderId);
            return SendResponseAsync(receiverId: receiverId, response);
        }

        public void Connect()
        {
            _p2PCommunicationService.OnP2PMessagesReceived += OnP2PMessagesReceived;
            _responseMessageHandler.OnDappConnected += _responseMessageHandler_OnDappConnected;
            _p2PCommunicationService.Start();

            Connected = _p2PCommunicationService.Syncing;
        }

        private void _responseMessageHandler_OnDappConnected(object sender, DappConnectedEventArgs e)
        {
            OnDappsListChanged?.Invoke(this, new DappConnectedEventArgs(e.dappMetadata, e.dappPermissionInfo));
        }

        public void Disconnect()
        {
            _p2PCommunicationService.Stop();
            _p2PCommunicationService.OnP2PMessagesReceived -= OnP2PMessagesReceived;
            _responseMessageHandler.OnDappConnected -= _responseMessageHandler_OnDappConnected;
            Connected = _p2PCommunicationService.Syncing;

            _logger.LogInformation("{@Sender}: disconnecting, is connected {Status}", "Beacon", Connected);
        }

        public async Task SendResponseAsync(string receiverId, BaseBeaconMessage response)
        {
            var peer = _peerRepository.TryReadAsync(receiverId).Result
                       ?? throw new NullReferenceException(nameof(Peer));

            var message = _responseMessageHandler.Handle(response, receiverId);

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
            (AcknowledgeResponse ack, BaseBeaconMessage requestMessage) =
                _requestMessageHandler.Handle(message, SenderId);

            if (requestMessage.Version != "1")
                await SendResponseAsync(requestMessage.SenderId, ack);

            bool hasPermission = await HasPermission(requestMessage);

            if (hasPermission)
                OnBeaconMessageReceived?.Invoke(this,
                    new BeaconMessageEventArgs(requestMessage.SenderId, requestMessage));
            else
                _logger.LogInformation("Received message have not permission");
        }

        private async Task<bool> HasPermission(BaseBeaconMessage beaconRequest)
        {
            switch (beaconRequest.Type)
            {
                case BeaconMessageType.permission_request:
                case BeaconMessageType.broadcast_request:
                    return true;

                case BeaconMessageType.operation_request:
                {
                    var request = beaconRequest as OperationRequest;
                    var permissionInfo =
                        await TryReadPermissionInfo(request!.SourceAddress, request.SenderId, request.Network);

                    return permissionInfo != null && permissionInfo.Scopes.Contains(PermissionScope.operation_request);
                }

                case BeaconMessageType.sign_payload_request:
                {
                    var permissionInfo = await PermissionInfoRepository.TryReadBySenderIdAsync(beaconRequest.SenderId);
                    return permissionInfo != null && permissionInfo.Scopes.Contains(PermissionScope.sign);
                }

                default:
                    return false;
            }
        }

        public async Task<PermissionInfo?> TryReadPermissionInfo(string sourceAddress, string senderId, Network network)
        {
            var accountId = AccountService.GetAccountId(sourceAddress, network);
            return await PermissionInfoRepository.TryReadAsync(senderId, accountId);
        }
    }
}