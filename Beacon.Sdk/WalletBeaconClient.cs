namespace Beacon.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core.Domain.Interfaces;
    using Core.Domain.Interfaces.Data;
    using Core.Transport.P2P;
    using Core.Transport.P2P.Dto.Handshake;
    using Matrix.Sdk.Core.Utils;
    using Sodium;
    
    public class WalletBeaconClient : IWalletBeaconClient
    {
        private readonly IP2PCommunicationClient _p2PCommunicationClient;
        private readonly IBeaconPeerRepository _beaconPeerRepository;
        private readonly ICryptographyService _cryptographyService;

        private KeyPair? _keyPair;

        public WalletBeaconClient(
            IP2PCommunicationClient p2PCommunicationClient,
            ICryptographyService cryptographyService,
            IBeaconPeerRepository beaconPeerRepository,
            WalletBeaconClientOptions options)
        {
            _cryptographyService = cryptographyService;
            _beaconPeerRepository = beaconPeerRepository;
            _p2PCommunicationClient = p2PCommunicationClient;

            AppName = options.AppName;
        }

        public event EventHandler<BeaconMessageEventArgs> OnBeaconMessageReceived;
        
        public string AppName { get; }

        public async Task InitAsync()
        {
            _keyPair = ReadBeaconSecret();

            await _p2PCommunicationClient.LoginAsync(_keyPair);
        }

        private void OnP2PMessagesReceived(object? sender, P2PMessageEventArgs e)
        {
            if (sender is not IP2PCommunicationClient)
                throw new ArgumentException("sender is not IP2PCommunicationClient");

            List<string> messages = e.Messages;
            
            
            OnBeaconMessageReceived.Invoke(this, new BeaconMessageEventArgs());
        }

        public async Task AddPeerAsync(P2PPairingRequest pairingRequest, bool sendPairingResponse = true)
        {
            if (!HexString.TryParse(pairingRequest.PublicKey, out HexString receiverPublicKeyHex))
                throw new InvalidOperationException("Can not parse receiver public key.");

            string pairingRequestId =
                pairingRequest.Id ?? throw new NullReferenceException("Provide pairing request id.");

            var version = int.Parse(pairingRequest.Version);

            _beaconPeerRepository.Create(pairingRequest.Name, receiverPublicKeyHex, pairingRequest.Version);

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

        private KeyPair ReadBeaconSecret()
        {
            // Todo: add storage
            if (_keyPair != null)
                return _keyPair;

            var seed = Guid.NewGuid().ToString();
            _keyPair = _cryptographyService.GenerateEd25519KeyPair(seed);

            return _keyPair;
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