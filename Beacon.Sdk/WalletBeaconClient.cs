namespace Beacon.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core.Beacon;
    using Core.Domain;
    using Core.Domain.Interfaces;
    using Core.Domain.Interfaces.Data;
    using Core.Domain.P2P;
    using Core.Domain.P2P.Dto.Handshake;
    using Core.Domain.Services;
    using Core.Infrastructure;
    using Core.Utils;
    using Sodium;

    /*
     * Todo: add AppMetadataRepository
     * 
     *
     */
    public class WalletBeaconClient : IWalletBeaconClient
    {
        private readonly IBeaconPeerRepository _beaconPeerRepository;
        private readonly ICryptographyService _cryptographyService;
        private readonly JsonSerializerService _jsonSerializerService;
        private readonly KeyPairService _keyPairService;
        private readonly IP2PCommunicationService _p2PCommunicationClient;

        public WalletBeaconClient(
            IP2PCommunicationService p2PCommunicationClient,
            IBeaconPeerRepository beaconPeerRepository,
            JsonSerializerService jsonSerializerService,
            KeyPairService keyPairService,
            ICryptographyService cryptographyService,
            WalletBeaconClientOptions options)
        {
            _beaconPeerRepository = beaconPeerRepository;
            _p2PCommunicationClient = p2PCommunicationClient;
            _jsonSerializerService = jsonSerializerService;
            _keyPairService = keyPairService;
            _cryptographyService = cryptographyService;

            AppName = options.AppName;
        }

        public event EventHandler<BeaconMessageEventArgs>? OnBeaconMessageReceived;

        public HexString BeaconId
        {
            get
            {
                KeyPair keyPair = _keyPairService.KeyPair;
                if (!HexString.TryParse(keyPair.PublicKey, out HexString result))
                    throw new InvalidOperationException("");

                return result;
            }
        }

        public string AppName { get; }

        public async Task InitAsync() => await _p2PCommunicationClient.LoginAsync();

        public async Task AddPeerAsync(P2PPairingRequest pairingRequest, bool sendPairingResponse = true)
        {
            if (!HexString.TryParse(pairingRequest.PublicKey, out HexString receiverHexPublicKey))
                throw new InvalidOperationException("Can not parse receiver public key.");

            string pairingRequestId =
                pairingRequest.Id ?? throw new NullReferenceException("Provide pairing request id.");

            BeaconPeer beaconPeer = BeaconPeer.Factory.Create(_cryptographyService, pairingRequest.Name,
                pairingRequest.RelayServer, receiverHexPublicKey, pairingRequest.Version);

            await _beaconPeerRepository.Create(beaconPeer);

            if (sendPairingResponse)
                await _p2PCommunicationClient.SendChannelOpeningMessageAsync(pairingRequestId, receiverHexPublicKey,
                    pairingRequest.RelayServer, pairingRequest.Version, AppName);
        }

        public async Task RespondAsync(BeaconBaseMessage beaconBaseMessage)
        {
            string message = _jsonSerializerService.Serialize(beaconBaseMessage);

            // BeaconPeer beaconPeer = _beaconPeerRepository.TryReadByUserId();
        }

        public void Connect()
        {
            _p2PCommunicationClient.OnP2PMessagesReceived += OnP2PMessagesReceived;
            _p2PCommunicationClient.Start();
        }

        public void Disconnect()
        {
            _p2PCommunicationClient.Stop();
            _p2PCommunicationClient.OnP2PMessagesReceived -= OnP2PMessagesReceived;
        }

        private void OnP2PMessagesReceived(object? sender, P2PMessageEventArgs e)
        {
            if (sender is not IP2PCommunicationService)
                throw new ArgumentException("sender is not IP2PCommunicationClient");

            List<string> messages = e.Messages;

            foreach (string message in messages)
            {
                BeaconBaseMessage beaconBaseMessage = _jsonSerializerService.Deserialize<BeaconBaseMessage>(message);

                OnBeaconMessageReceived?.Invoke(this, new BeaconMessageEventArgs(beaconBaseMessage));
            }
        }

        private string GetSenderId() => string.Empty;

        private async Task SendAcknowledgeResponseAsync(BeaconBaseMessage beaconBaseMessage)
        {
            var acknowledgeResponse =
                new AcknowledgeResponse(BeaconConstants.BeaconVersion, beaconBaseMessage.Id, GetSenderId());
        }
    }
}