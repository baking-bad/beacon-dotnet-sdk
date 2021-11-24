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
    using Core.Utils;
    using Microsoft.Extensions.Logging;
    using Sodium;

    /*
     * Todo: add AppMetadataRepository
     * 
     *
     */
    public class WalletBeaconClient : IWalletBeaconClient
    {
        private readonly ILogger<WalletBeaconClient> _logger;
        private readonly IBeaconPeerRepository _beaconPeerRepository;
        private readonly IPeerRoomRepository _peerRoomRepository;
        private readonly ICryptographyService _cryptographyService;
        private readonly IP2PCommunicationService _p2PCommunicationClient;
        private readonly IJsonSerializerService _jsonSerializerService;
        
        private readonly KeyPairService _keyPairService;

        public WalletBeaconClient(
            ILogger<WalletBeaconClient> logger, 
            IBeaconPeerRepository beaconPeerRepository, 
            IPeerRoomRepository peerRoomRepository, 
            ICryptographyService cryptographyService, 
            IP2PCommunicationService p2PCommunicationClient, 
            IJsonSerializerService jsonSerializerService, 
            WalletBeaconClientOptions options)
        {
            _logger = logger;
            _beaconPeerRepository = beaconPeerRepository;
            _peerRoomRepository = peerRoomRepository;
            _cryptographyService = cryptographyService;
            _p2PCommunicationClient = p2PCommunicationClient;
            _jsonSerializerService = jsonSerializerService;
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
            {
                _logger.LogError("Can not parse receiver public key");
                return;
            }
            
            BeaconPeer beaconPeer = BeaconPeer.Factory.Create(
                _cryptographyService,
                pairingRequest.Name,
                pairingRequest.RelayServer, 
                receiverHexPublicKey, 
                pairingRequest.Version);

            beaconPeer = _beaconPeerRepository.Create(beaconPeer).Result;
            
            if (sendPairingResponse)
                await _p2PCommunicationClient.SendChannelOpeningMessageAsync(pairingRequest.Id, beaconPeer, AppName);
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

            BeaconPeer beaconPeer = _beaconPeerRepository.TryReadByUserId(beaconBaseMessage.SenderId).Result 
                                    ?? throw new NullReferenceException(nameof(BeaconPeer));

            BeaconPeerRoom beaconPeerRoom = _peerRoomRepository.TryRead(beaconPeer.HexPublicKey).Result  
                                            ?? throw new NullReferenceException(nameof(BeaconPeerRoom));;
            
            string message = _jsonSerializerService.Serialize(acknowledgeResponse);

            await _p2PCommunicationClient.SendMessageAsync(beaconPeerRoom.RoomId, message);
        }
    }
}