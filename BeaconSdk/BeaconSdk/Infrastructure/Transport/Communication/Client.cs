// ReSharper disable SuggestVarOrType_BuiltInTypes

namespace BeaconSdk.Infrastructure.Transport.Communication
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Cryptography;
    using Domain.Beacon.P2P;
    using MatrixSdk.Application;
    using MatrixSdk.Application.Listener;
    using MatrixSdk.Domain.Room;
    using MatrixSdk.Utils;
    using Sodium;

    public class Client
    {
        private static readonly Dictionary<HexString, MatrixEventListener<List<BaseRoomEvent>>> EncryptedMessageListeners = new();
        private readonly string _appName;
        private readonly MatrixClient _matrixClient;
        private KeyPair? _keyPair;

        public Client(MatrixClient matrixClient, string appName)
        {
            _matrixClient = matrixClient;
            _appName = appName;

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
                if (!HexString.TryParse(peer.PublicKey, out var publicKeyHex))
                    throw new InvalidOperationException("Can not parse peer.PublicKey");

                var recipientId = ClientHelper.CreateRecipientId(peer.RelayServer, publicKeyHex);

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

                var pairingPayloadMessage = ClientHelper.CreatePairingPayload(peer, _keyPair!.PublicKey, "beacon-node-0.papers.tech:8448", _appName);

                
                var t = PublicKeyAuth.ConvertEd25519PublicKeyToCurve25519PublicKey(publicKeyHex.ToByteArray());
                // var payload = BeaconCryptographyService.EncryptAsHex(pairingPayloadMessage, t).ToString();

                var payload = SealedPublicKeyBox.Create(pairingPayloadMessage, recipientPublicKey: t);

                if (!HexString.TryParse(payload, out var result))
                    throw new Exception("");
                
                var channelOpeningMessage = ClientHelper.GetChannelOpeningMessage(recipientId, result.Value);

                // var pairingResponseAggregate = ClientMessageFactory.CreatePairingResponse(peer, _keyPair!, _appName);
                //

                var eventId = await _matrixClient.SendMessageAsync(matrixRoom.Id, channelOpeningMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}