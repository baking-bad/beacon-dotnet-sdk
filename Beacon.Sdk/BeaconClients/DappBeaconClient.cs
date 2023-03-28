namespace Beacon.Sdk.BeaconClients
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using Abstract;
    using Beacon;
    using Beacon.Operation;
    using Beacon.Permission;
    using Beacon.Sign;
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
            DeserializeMessageHandler deserializeMessageHandler,
            SerializeMessageHandler serializeMessageHandler,
            BeaconOptions options)
            : base(keyPairService,
                accountService,
                appMetadataRepository,
                permissionInfoRepository,
                seedRepository,
                p2PCommunicationService,
                peerRepository,
                serializeMessageHandler,
                deserializeMessageHandler,
                jsonSerializerService,
                options)
        {
            _logger = logger;
        }

        public string GetPairingRequestInfo()
        {
            var pairingRequestInfo = P2PCommunicationService
                .GetPairingRequestInfo(AppName, KnownRelayServers, IconUrl, AppUrl)
                .Result;
            var pairingString = JsonSerializerService.Serialize(pairingRequestInfo);
            var pairingBytes = Encoding.UTF8.GetBytes(pairingString);
            var pairingQrCode = Base58.Convert(pairingBytes);
            return pairingQrCode;
        }

        public Peer? GetActivePeer()
        {
            return PeerRepository.TryGetActive().Result;
        }

        public void RemoveActiveAccounts()
        {
            PeerRepository.MarkAllInactive();
        }

        public PermissionInfo? GetActiveAccount()
        {
            var activePeer = GetActivePeer();
            if (activePeer == null) return null;

            return PermissionInfoRepository
                .TryReadBySenderIdAsync(activePeer.SenderId)
                .Result;
        }

        public Task RequestPermissions(IEnumerable<PermissionScope> permissions, Network network)
        {
            var activePeer = GetActivePeer();
            if (activePeer == null) return Task.CompletedTask;

            var permissionRequest = new PermissionRequest(
                type: BeaconMessageType.permission_request,
                version: Constants.BeaconVersion,
                id: KeyPairService.CreateGuid(),
                senderId: SenderId,
                appMetadata: Metadata,
                network: network,
                scopes: new List<PermissionScope>(permissions)
            );

            return SendResponseAsync(activePeer.SenderId, permissionRequest);
        }

        public Task RequestOperation(IEnumerable<TezosBaseOperation> operations)
        {
            var activeAccount = GetActiveAccount();
            if (activeAccount == null) return Task.CompletedTask;

            var operationRequest = new OperationRequest(
                type: BeaconMessageType.operation_request,
                version: Constants.BeaconVersion,
                id: KeyPairService.CreateGuid(),
                senderId: SenderId,
                network: activeAccount.Network,
                operationDetails: new List<TezosBaseOperation>(operations),
                sourceAddress: activeAccount.Address);

            return SendResponseAsync(activeAccount.SenderId, operationRequest);
        }

        public Task RequestSign(string payload, SignPayloadType payloadType)
        {
            var activeAccount = GetActiveAccount();
            if (activeAccount == null) return Task.CompletedTask;

            var signPayloadRequest = new SignPayloadRequest(
                id: KeyPairService.CreateGuid(),
                version: Constants.BeaconVersion,
                senderId: SenderId,
                signingType: payloadType,
                payload: payload,
                sourceAddress: activeAccount.Address);

            return SendResponseAsync(activeAccount.SenderId, signPayloadRequest);
        }

        protected override async Task OnP2PMessagesReceived(object? sender, P2PMessageEventArgs e)
        {
            if (sender is not IP2PCommunicationService)
                throw new ArgumentException("sender is not IP2PCommunicationClient");

            if (e.PairingResponse != null)
            {
                var peer = PeerRepository.TryGetActive().Result;
                if (peer == null) return;

                var appMetaData = new AppMetadata
                {
                    SenderId = peer.SenderId,
                    Name = e.PairingResponse.Name,
                    Icon = e.PairingResponse.Icon,
                    AppUrl = e.PairingResponse.AppUrl
                };
                await AppMetadataRepository.CreateOrUpdateAsync(appMetaData);

                RaiseOnBeaconMessageReceived(new BeaconMessageEventArgs(null, true));
                return;
            }

            foreach (var message in e.Messages)
                await HandleReceivedMessage(message);
        }

        private async Task HandleReceivedMessage(string message)
        {
            var (_, receivedMessage) =
                DeserializeMessageHandler.Handle(message, SenderId);

            if (receivedMessage.Type == BeaconMessageType.permission_response)
                await SerializeMessageHandler.Handle(receivedMessage, receivedMessage.SenderId);

            if (receivedMessage.Type == BeaconMessageType.disconnect)
                await RemovePeerAsync(receivedMessage.SenderId);

            RaiseOnBeaconMessageReceived(new BeaconMessageEventArgs(receivedMessage));
        }
    }
}