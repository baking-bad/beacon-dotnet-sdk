using System.Text;
using Netezos.Encoding;

namespace Beacon.Sdk.BeaconClients
{
    using System;
    using System.Threading.Tasks;
    using Abstract;
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
        private readonly PeerFactory _peerFactory;

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
            DeserializeMessageHandler deserializeMessageHandler,
            SerializeMessageHandler serializeMessageHandler,
            IJsonSerializerService jsonSerializerService,
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
            _peerFactory = peerFactory;
        }

        public async Task AddPeerAsync(P2PPairingRequest pairingRequest, bool sendPairingResponse = true)
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
                pairingRequest.RelayServer
            );

            peer = PeerRepository.CreateAsync(peer).Result;

            if (sendPairingResponse)
                await P2PCommunicationService
                    .SendChannelOpeningMessageAsync(peer, pairingRequest.Id, AppName, AppUrl, IconUrl);
        }

        protected override async Task OnP2PMessagesReceived(object? sender, P2PMessageEventArgs e)
        {
            if (sender is not IP2PCommunicationService)
                throw new ArgumentException("sender is not IP2PCommunicationClient");

            foreach (string message in e.Messages)
                await HandleMessage(message);
        }

        private async Task HandleMessage(string message)
        {
            (AcknowledgeResponse ack, BaseBeaconMessage requestMessage) =
                DeserializeMessageHandler.Handle(message, SenderId);

            if (requestMessage.Version != "1")
                await SendResponseAsync(requestMessage.SenderId, ack);

            var hasPermission = await HasPermission(requestMessage);

            if (hasPermission)
                RaiseOnBeaconMessageReceived(new BeaconMessageEventArgs(requestMessage));
            else
                _logger.LogError("Received message have not permission");
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

        public P2PPairingRequest GetPairingRequest(string pairingData)
        {
            var decodedPairingData = Base58.Parse(pairingData);
            var stringPairingData = Encoding.UTF8.GetString(decodedPairingData);
            return JsonSerializerService.Deserialize<P2PPairingRequest>(stringPairingData);
        }
    }
}