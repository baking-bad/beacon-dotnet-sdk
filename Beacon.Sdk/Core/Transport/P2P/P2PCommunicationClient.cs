namespace Beacon.Sdk.Core.Transport.P2P
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using ChannelOpening;
    using Domain.Interfaces;
    using Matrix.Sdk;
    using Matrix.Sdk.Core.Domain.MatrixRoom;
    using Matrix.Sdk.Core.Domain.Room;
    using Matrix.Sdk.Core.Utils;
    using Sodium;
    using SodiumCore = Sodium.SodiumCore;

    public class P2PCommunicationCommunicationClient : IP2PCommunicationClient
    {
        public event EventHandler<P2PMessageEventArgs> OnP2PMessagesReceived;
        
        private readonly EventService _eventService;
        private readonly IChannelOpeningMessageBuilder _channelOpeningMessageBuilder;
        private readonly IMatrixClient _matrixClient;
        private readonly RelayServerService _relayServerService;
        private KeyPair? _keyPair;

        public P2PCommunicationCommunicationClient(
            EventService eventService,
            IMatrixClient matrixClient,
            IChannelOpeningMessageBuilder channelOpeningMessageBuilder,
            RelayServerService relayServerService)
        {
            _eventService = eventService;
            _matrixClient = matrixClient;
            _channelOpeningMessageBuilder = channelOpeningMessageBuilder;
            _relayServerService = relayServerService;

            //Todo: refactor
            SodiumCore.Init();
        }

        public async Task LoginAsync(KeyPair keyPair)
        {
            _keyPair = keyPair;
            string relayServer = await _relayServerService.GetRelayServer(keyPair.PublicKey);

            await _matrixClient.LoginAsync(new Uri(relayServer), keyPair);
        }

        public void Start()
        {
            _matrixClient.OnMatrixRoomEventsReceived += OnMatrixRoomEventsReceived;
            _matrixClient.Start();
        }

        private void OnMatrixRoomEventsReceived(object? sender, MatrixRoomEventsEventArgs e)
        {
            if (sender is not IMatrixClient)
                throw new ArgumentException("sender is not IMatrixClient");
            
            List<BaseRoomEvent> matrixRoomEvents = e.MatrixRoomEvents;
            var messages = new List<string>();
            
            foreach (BaseRoomEvent matrixRoomEvent in matrixRoomEvents)
            {
                KeyPair keyPair = _keyPair ?? throw new NullReferenceException("no keyPair");
                string? message = _eventService.TryDecryptMessageFromEvent(matrixRoomEvent, keyPair);
                
                if (message != null)
                    messages.Add(message);
            }
            
            OnP2PMessagesReceived.Invoke(this, new P2PMessageEventArgs(messages));
        }

        public void Stop()
        {
            _matrixClient.OnMatrixRoomEventsReceived -= OnMatrixRoomEventsReceived;
            _matrixClient.Stop();
        }

        public async Task SendChannelOpeningMessageAsync(string id, HexString receiverHexPublicKey,
            string receiverRelayServer, int version, string appName)
        {
            try
            {
                if (!HexString.TryParse(_keyPair!.PublicKey, out HexString senderHexPublicKey))
                    throw new InvalidOperationException("Can not parse sender public key.");

                var senderRelayServer =
                    (_matrixClient.BaseAddress ?? throw new NullReferenceException("Provide P2PClient BaseAddress")).ToString();

                _channelOpeningMessageBuilder.Reset();
                _channelOpeningMessageBuilder.BuildRecipientId(receiverRelayServer, receiverHexPublicKey);
                _channelOpeningMessageBuilder.BuildPairingPayload(id, version, senderHexPublicKey, senderRelayServer, appName);
                _channelOpeningMessageBuilder.BuildEncryptedPayload(receiverHexPublicKey);

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

// private static readonly Dictionary<HexString, MatrixEventListener<List<BaseRoomEvent>>?>
//     CachedEncryptedMessageListeners = new();


// if (matrixRoomEvent is not TextMessageEvent textMessageEvent) continue;
//
// string senderUserId = textMessageEvent.SenderUserId;
// BeaconPeer? peer = _beaconPeerRepository.TryReadByUserId(senderUserId);
//
// if (peer == null) continue;
// if (!senderUserId.StartsWith(peer.UserId) ||
//     !_cryptographyService.Validate(textMessageEvent.Message)) continue;
//
// KeyPair keyPair = _keyPair ?? throw new NullReferenceException("no keyPair");
// SessionKeyPair serverSessionKeyPair =
//     _sessionKeyPairRepository.CreateOrReadServer(peer.HexPublicKey, keyPair);
//     
// string message = _cryptographyService.DecryptAsString(textMessageEvent.Message,
//     serverSessionKeyPair.Rx);


// If the listener is already registered, we do nothing.
// if (CachedEncryptedMessageListeners.TryGetValue(hexPublicKey, out _))
//     return;
//
// KeyPair keyPair = _keyPair ?? throw new NullReferenceException("_keyPair");
//
// SessionKeyPair serverSessionKeyPair =
//     _sessionKeyPairRepository.CreateOrReadServer(hexPublicKey, keyPair);
//
// EncryptedMessageListener listener =
//     _matrixEventListenerFactory.CreateEncryptedMessageListener(hexPublicKey, textMessageEvent =>
//     {
//         string message =
//             _cryptographyService.DecryptAsString(textMessageEvent.Message,
//                 serverSessionKeyPair.Rx);
//
//         messageCallback(message);
//     });
//
// // listener.ListenTo(_matrixClient.MatrixEventNotifier);
// CachedEncryptedMessageListeners[hexPublicKey] = listener;

// if (CachedEncryptedMessageListeners.TryGetValue(hexPublicKey,
//         out MatrixEventListener<List<BaseRoomEvent>>? listener))
//     listener?.Unsubscribe();