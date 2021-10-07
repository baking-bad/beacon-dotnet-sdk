// ReSharper disable SuggestVarOrType_BuiltInTypes

namespace BeaconSdk.Infrastructure.Transport.Communication
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Cryptography;
    using Domain.Beacon.P2P;
    using Domain.Pairing;
    using MatrixSdk.Application;
    using MatrixSdk.Application.Listener;
    using MatrixSdk.Domain.Room;
    using MatrixSdk.Utils;
    using Newtonsoft.Json;
    using Serialization;
    using Sodium;

    public record ClientOptions(string AppName);

    public class Client
    {
        private static readonly Dictionary<HexString, MatrixEventListener<List<BaseRoomEvent>>>
            EncryptedMessageListeners = new();

        private readonly string _appName;

        private readonly JsonSerializerService _jsonSerializerService;
        private readonly MatrixClient _matrixClient;
        private KeyPair? _keyPair;

        public Client(ClientOptions clientOptions, MatrixClient matrixClient,
            JsonSerializerService jsonSerializerService)
        {
            _matrixClient = matrixClient;
            _jsonSerializerService = jsonSerializerService;

            _appName = clientOptions.AppName;

            //Todo: refactor
            SodiumCore.Init();
        }

        // var seed = Guid.NewGuid().ToString();
        // var keyPair = cryptoService.GenerateKeyPairFromSeed(seed);
        public async Task StartAsync(KeyPair keyPair)
        {
            try
            {
                await _matrixClient.StartAsync(keyPair);

                _keyPair = keyPair;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void ListenToPublicKeyHex(HexString publicKey)
        {
            if (EncryptedMessageListeners.TryGetValue(publicKey, out _))
                return;

            // ReSharper disable once ArgumentsStyleNamedExpression
            var listener = new EncryptedMessageListener(_keyPair!, publicKey, e => { });

            EncryptedMessageListeners[publicKey] = listener;

            listener.ListenTo(_matrixClient.MatrixEventNotifier);
        }

        public void RemoveListenerForPublicKey(HexString publicKey)
        {
            if (EncryptedMessageListeners.TryGetValue(publicKey, out var listener))
                listener.Unsubscribe();
        }

        public async Task SendPairingResponseAsync(BeaconPeer peer)
        {
            try
            {
                if (!HexString.TryParse(peer.PublicKey, out var recipientHexPublicKey))
                    throw new InvalidOperationException("Can not parse peer.PublicKey");

                var recipientId = ClientHelper.CreateRecipientId(peer.RelayServer, recipientHexPublicKey);

                // We force room creation here because if we "re-pair", we need to make sure that we don't send it to an old room.
                var matrixRoom = await _matrixClient.CreateTrustedPrivateRoomAsync(new[]
                {
                    recipientId
                });

                // await this.updatePeerRoom(recipient, roomId)

                // Before we send the message, we have to wait for the join to be accepted.
                var spin = new SpinWait();
                while (_matrixClient.JoinedRooms.Length == 0)
                    spin.SpinOnce();

                if (!HexString.TryParse(_keyPair!.PublicKey, out var hexPublicKey))
                    throw new InvalidOperationException("Can not parse publicKey");
                
                var relayServer = "beacon-node-0.papers.tech:8448";
                var pairingPayloadMessage = CreatePairingPayloadV2(peer, hexPublicKey, relayServer, _appName);

                var recipientPublicKey = PublicKeyAuth.ConvertEd25519PublicKeyToCurve25519PublicKey(recipientHexPublicKey.ToByteArray());
                var payload = SealedPublicKeyBox.Create(pairingPayloadMessage, recipientPublicKey);
            
                if (!HexString.TryParse(payload, out var hexPayload))
                    throw new InvalidOperationException("Can not parse payload");
                
                var channelOpeningMessage = ClientHelper.GetChannelOpeningMessage(recipientId, hexPayload.Value);

                var eventId = await _matrixClient.SendMessageAsync(matrixRoom.Id, channelOpeningMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
        
        private string CreatePairingPayloadV2(BeaconPeer peer, HexString hexPublicKey, string relayServer, string appName)
        {
            if (peer.Id == null)
                throw new ArgumentNullException(nameof(peer.Id));

            var pairingResponse = new PairingResponse(
                peer.Id,
                "p2p-pairing-response",
                appName,
                "2",
                hexPublicKey.Value,
                relayServer);

            return _jsonSerializerService.Serialize(pairingResponse);
        }
    }
}

// var publicKey = BeaconCryptographyService.ToHexString(_keyPair.PublicKey);
// var m = publicKey == d.Value;