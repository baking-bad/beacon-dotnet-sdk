namespace Beacon.Sdk
{
    using System;
    using System.Threading.Tasks;
    using Core;
    using Core.Domain.Interfaces;
    using Core.Domain.Interfaces.Data;
    using Core.Transport.P2P;
    using Core.Transport.P2P.Dto.Handshake;
    using Matrix.Sdk.Core.Utils;
    using Sodium;
    

    public interface IWalletBeaconClient
    {
        string AppName { get; }

        Task StartAsync();

        Task AddPeerAsync(P2PPairingRequest pairingRequest, bool sendPairingResponse = true);

        void Connect();
    }

    public class WalletBeaconClient : IWalletBeaconClient
    {
        private readonly IBeaconPeerRepository _beaconPeerRepository;
        private readonly ICryptographyService _cryptographyService;
        private readonly IP2PClient _p2PClient;

        private KeyPair? _keyPair;

        public WalletBeaconClient(
            ICryptographyService cryptographyService,
            IP2PClient p2PClient,
            IBeaconPeerRepository beaconPeerRepository,
            ClientOptions options)
        {
            _cryptographyService = cryptographyService;
            _p2PClient = p2PClient;
            _beaconPeerRepository = beaconPeerRepository;

            AppName = options.AppName;
        }

        public string AppName { get; }

        public async Task StartAsync()
        {
            _keyPair = ReadBeaconSecret();
            await _p2PClient.StartAsync(_keyPair);
        }

        public async Task AddPeerAsync(P2PPairingRequest pairingRequest, bool sendPairingResponse = true)
        {
            if (!HexString.TryParse(pairingRequest.PublicKey, out HexString receiverPublicKeyHex))
                throw new InvalidOperationException("Can not parse receiver public key.");

            string pairingRequestId =
                pairingRequest.Id ?? throw new NullReferenceException("Provide pairing request id.");

            var version = int.Parse(pairingRequest.Version);

            _beaconPeerRepository.CreateOrUpdate(receiverPublicKeyHex, pairingRequest.Version);
            // var beaconPeer = new BeaconPeer(pairingRequest.Name, receiverPublicKeyHex, pairingRequest.Version);
            // _beaconPeerRepository.CreateOrUpdate(beaconPeer.HexPublicKey.ToString(), beaconPeer);

            // _p2PClient.ListenToHexPublicKey();
            if (sendPairingResponse)
                await _p2PClient.SendChannelOpeningMessageAsync(pairingRequestId, receiverPublicKeyHex,
                    pairingRequest.RelayServer, version);
        }

        public void Connect()
        {
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