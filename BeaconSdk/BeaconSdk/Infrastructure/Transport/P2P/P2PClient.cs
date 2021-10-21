// ReSharper disable SuggestVarOrType_BuiltInTypes

namespace BeaconSdk.Infrastructure.Transport.P2P
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using ChannelOpening;
    using Domain.Beacon.P2P;
    using MatrixSdk.Application;
    using MatrixSdk.Application.Listener;
    using MatrixSdk.Domain;
    using MatrixSdk.Domain.Room;
    using MatrixSdk.Utils;
    using Sodium;

    public record ClientOptions(string AppName, string RelayServer);

    public class P2PClient
    {
        private readonly string _appName;
        private readonly string _relayServer;
        private readonly MatrixClient _matrixClient;
        private readonly IChannelOpeningMessageBuilder _channelOpeningMessageBuilder;
        
        private static readonly Dictionary<HexString, MatrixEventListener<List<BaseRoomEvent>>>
            EncryptedMessageListeners = new();
        
        private KeyPair? _keyPair;

        public P2PClient(MatrixClient matrixClient, IChannelOpeningMessageBuilder channelOpeningMessageBuilder, ClientOptions options)
        {
            _matrixClient = matrixClient;
            _channelOpeningMessageBuilder = channelOpeningMessageBuilder;

            _appName = options.AppName;
            _relayServer = options.RelayServer;

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
            listener.ListenTo(_matrixClient.MatrixEventNotifier);
            
            EncryptedMessageListeners[publicKey] = listener;
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
                if (!HexString.TryParse(peer.PublicKey, out var receiverPublicKeyHex))
                    throw new InvalidOperationException("Can not parse receiver public key.");
                
                _channelOpeningMessageBuilder.Reset();
                _channelOpeningMessageBuilder.BuildRecipientId(peer.RelayServer, receiverPublicKeyHex);
                
                var pairingRequestId = peer.Id ?? throw new NullReferenceException("Provide pairing request id.");
                var payloadVersion = int.Parse(peer.Version);
               
                if (!HexString.TryParse(_keyPair!.PublicKey, out var senderPublicKeyHex))
                    throw new InvalidOperationException("Can not parse sender public key.");
                
                _channelOpeningMessageBuilder.BuildPairingPayload(pairingRequestId, payloadVersion, senderPublicKeyHex, _relayServer, _appName);
                _channelOpeningMessageBuilder.BuildEncryptedPayload(receiverPublicKeyHex);
                
                ChannelOpeningMessage channelOpeningMessage = _channelOpeningMessageBuilder.Message;

                // We force room creation here because if we "re-pair", we need to make sure that we don't send it to an old room.
                MatrixRoom matrixRoom = await _matrixClient.CreateTrustedPrivateRoomAsync(new[]
                {
                    channelOpeningMessage.RecipientId
                });
                
                var spin = new SpinWait();
                while (_matrixClient.JoinedRooms.Length == 0)
                    spin.SpinOnce();
                
                _ = await _matrixClient.SendMessageAsync(matrixRoom.Id, channelOpeningMessage.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}