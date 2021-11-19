namespace Beacon.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core.Domain.Interfaces;
    using Core.Domain.Interfaces.Data;
    using Core.Infrastructure.Serialization;
    using Core.Transport.P2P;
    using Core.Transport.P2P.Dto.Handshake;
    using Core.Utils;


    public class WalletBeaconClient : IWalletBeaconClient
    {
        private readonly IP2PCommunicationService _p2PCommunicationClient;
        private readonly IBeaconPeerRepository _beaconPeerRepository;
        private readonly JsonSerializerService _jsonSerializerService;

        public WalletBeaconClient(
            IP2PCommunicationService p2PCommunicationClient,
            IBeaconPeerRepository beaconPeerRepository, 
            JsonSerializerService jsonSerializerService,
            WalletBeaconClientOptions options)
        {
            _beaconPeerRepository = beaconPeerRepository;
            _p2PCommunicationClient = p2PCommunicationClient;
            _jsonSerializerService = jsonSerializerService;

            AppName = options.AppName;
        }

        public event EventHandler<BeaconMessageEventArgs>? OnBeaconMessageReceived;
        
        public string AppName { get; }

        public async Task InitAsync() => await _p2PCommunicationClient.LoginAsync();

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

        public async Task AddPeerAsync(P2PPairingRequest pairingRequest, bool sendPairingResponse = true)
        {
            if (!HexString.TryParse(pairingRequest.PublicKey, out HexString receiverPublicKeyHex))
                throw new InvalidOperationException("Can not parse receiver public key.");

            string pairingRequestId =
                pairingRequest.Id ?? throw new NullReferenceException("Provide pairing request id.");

            var version = int.Parse(pairingRequest.Version);

            _beaconPeerRepository.Create(pairingRequest.Name, pairingRequest.RelayServer, receiverPublicKeyHex, pairingRequest.Version);

            if (sendPairingResponse)
                await _p2PCommunicationClient.SendChannelOpeningMessageAsync(pairingRequestId, receiverPublicKeyHex,
                    pairingRequest.RelayServer, version, AppName);
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
    }
}

// var beaconPeer = new BeaconPeer(pairingRequest.Name, receiverPublicKeyHex, pairingRequest.Version);
// _beaconPeerRepository.CreateOrUpdate(beaconPeer.HexPublicKey.ToString(), beaconPeer);

// _p2PClient.ListenToHexPublicKey();

// public async Task AddPeerAsync(P2PPairingRequest pairingRequest, bool withPairingResponse = true)
// {
//     if (!HexString.TryParse(pairingRequest.PublicKey, out HexString receiverPublicKeyHex))
//         throw new InvalidOperationException("Can not parse receiver public key.");
//
//     string pairingRequestId =
//         pairingRequest.Id ?? throw new NullReferenceException("Provide pairing request id.");
//     var version = int.Parse(pairingRequest.Version);
//
//     var pairingResponse = new PairingResponse(
//         pairingRequestId,
//         receiverPublicKeyHex,
//         pairingRequest.RelayServer,
//         version);
//
//     if (withPairingResponse)
//         await _p2PClient.SendChannelOpeningMessageAsync(pairingResponse);
// }