namespace Beacon.Sdk.Core.Transport.P2P
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using ChannelOpening;
    using Dto.Handshake;
    using Infrastructure.Cryptography.Libsodium;
    using Infrastructure.Repositories;
    using Matrix.Sdk;
    using Matrix.Sdk.Core.Domain.MatrixRoom;
    using Matrix.Sdk.Core.Domain.Room;
    using Matrix.Sdk.Core.Utils;
    using Matrix.Sdk.Listener;
    using Sodium;
    using SodiumCore = Sodium.SodiumCore;

    // public class EventListenerRepository
    // {
    //     private readonly IEventListenerFactory _eventListenerFactory;
    // }

    public record ClientOptions(string AppName);

    public class P2PClient : IP2PClient
    {
        private static readonly Dictionary<HexString, MatrixEventListener<List<BaseRoomEvent>>>
            CachedEncryptedMessageListeners = new();

        private readonly IChannelOpeningMessageBuilder _channelOpeningMessageBuilder;
        private readonly ICryptographyService _cryptographyService;
        private readonly IEventListenerFactory _eventListenerFactory;
        private readonly IMatrixClient _matrixClient;

        private readonly RelayServerService _relayServerService;
        private readonly ISessionKeyPairRepository _sessionKeyPairRepository;
        private KeyPair? _keyPair;

        public P2PClient(
            IMatrixClient matrixClient,
            IChannelOpeningMessageBuilder channelOpeningMessageBuilder,
            IEventListenerFactory eventListenerFactory,
            ISessionKeyPairRepository sessionKeyPairRepository,
            ICryptographyService cryptographyService,
            RelayServerService relayServerService, ClientOptions options)
        {
            _matrixClient = matrixClient;
            _channelOpeningMessageBuilder = channelOpeningMessageBuilder;
            _eventListenerFactory = eventListenerFactory;
            _sessionKeyPairRepository = sessionKeyPairRepository;
            _cryptographyService = cryptographyService;
            _relayServerService = relayServerService;

            AppName = options.AppName;

            //Todo: refactor
            SodiumCore.Init();
        }

        public string AppName { get; private set; }

        public Uri? BaseAddress { get; private set; }

        public async Task StartAsync(KeyPair keyPair)
        {
            try
            {
                _keyPair = keyPair;

                string relayServer = await _relayServerService.GetRelayServer(keyPair.PublicKey);
                BaseAddress = new Uri(relayServer);

                await _matrixClient.StartAsync(BaseAddress, keyPair);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task StopAsync()
        {
            await _matrixClient.Stop();
        }

        public void ListenToHexPublicKey(HexString hexPublicKey, Action<string> messageCallback)
        {
            // If the listener is already registered, we do nothing.
            if (CachedEncryptedMessageListeners.TryGetValue(hexPublicKey, out _))
                return;
            
            KeyPair keyPair = _keyPair ?? throw new NullReferenceException("_keyPair")
            SessionKeyPair serverSessionKeyPair =
                _sessionKeyPairRepository.CreateOrReadServer(hexPublicKey, keyPair);

            EncryptedMessageListener listener =
                _eventListenerFactory.CreateEncryptedMessageListener(keyPair, hexPublicKey, textMessageEvent =>
                {
                    string message =
                        _cryptographyService.DecryptAsString(textMessageEvent.Message,
                            serverSessionKeyPair.Rx);

                    messageCallback(message);
                });

            listener.ListenTo(_matrixClient.MatrixEventNotifier);
            CachedEncryptedMessageListeners[hexPublicKey] = listener;
        }

        public void RemoveListenerForPublicKey(HexString hexPublicKey)
        {
            if (CachedEncryptedMessageListeners.TryGetValue(hexPublicKey, out MatrixEventListener<List<BaseRoomEvent>> listener))
                listener.Unsubscribe();
        }
        
        public async Task SendChannelOpeningMessageAsync(string id, HexString receiverHexPublicKey, string receiverRelayServer, int version)
        {
            try
            {
                if (!HexString.TryParse(_keyPair!.PublicKey, out HexString senderHexPublicKey))
                    throw new InvalidOperationException("Can not parse sender public key.");

                var senderRelayServer = (BaseAddress ?? throw new NullReferenceException("Provide P2PClient BaseAddress")).ToString();
                
                _channelOpeningMessageBuilder.Reset();
                _channelOpeningMessageBuilder.BuildRecipientId(receiverRelayServer, receiverHexPublicKey);
                _channelOpeningMessageBuilder.BuildPairingPayload(id, version, senderHexPublicKey, senderRelayServer, AppName);
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