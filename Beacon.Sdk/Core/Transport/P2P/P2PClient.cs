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

    public reacord P

    public class P2PClient // : IP2PClient
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
            RelayServerService relayServerService)
        {
            _matrixClient = matrixClient;
            _channelOpeningMessageBuilder = channelOpeningMessageBuilder;
            _eventListenerFactory = eventListenerFactory;
            _sessionKeyPairRepository = sessionKeyPairRepository;
            _cryptographyService = cryptographyService;
            _relayServerService = relayServerService;

            //Todo: refactor
            SodiumCore.Init();
        }

        public string? Name { get; private set; }

        public Uri? BaseAddress { get; private set; }

        public async Task StartAsync(string name, KeyPair keyPair)
        {
            try
            {
                Name = name;
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
            await _matrixClient.StopAsync();
        }

        public void ListenToPublicKeyHex(HexString senderPublicKeyHex, Action<string> messageCallback)
        {
            // If the listener is already registered, we do nothing.
            if (CachedEncryptedMessageListeners.TryGetValue(senderPublicKeyHex, out _))
                return;

            // In the context of this method, there are sender and receiver, so we need a proper naming.
            KeyPair receiverKeyPair = _keyPair ?? throw new NullReferenceException("_keyPair");
            SessionKeyPair serverSessionKeyPair =
                _sessionKeyPairRepository.CreateOrReadServer(senderPublicKeyHex, receiverKeyPair);

            EncryptedMessageListener listener =
                _eventListenerFactory.CreateEncryptedMessageListener(_keyPair, senderPublicKeyHex, textMessageEvent =>
                {
                    string message =
                        _cryptographyService.DecryptAsString(textMessageEvent.Message,
                            serverSessionKeyPair.Rx);

                    messageCallback(message);
                });

            listener.ListenTo(_matrixClient.MatrixEventNotifier);
            CachedEncryptedMessageListeners[senderPublicKeyHex] = listener;
        }

        public void RemoveListenerForPublicKey(HexString publicKey)
        {
            if (CachedEncryptedMessageListeners.TryGetValue(publicKey,
                    out MatrixEventListener<List<BaseRoomEvent>> listener))
                listener.Unsubscribe();
        }


        public async Task SendPairingResponseAsync(PairingResponse response)
        {
            try
            {
                if (!HexString.TryParse(_keyPair!.PublicKey, out HexString senderHexPublicKey))
                    throw new InvalidOperationException("Can not parse sender public key.");

                string senderAppName = Name ?? throw new NullReferenceException("Provide P2PClient Name");

                _channelOpeningMessageBuilder.Reset();
                _channelOpeningMessageBuilder.BuildRecipientId(response.ReceiverRelayServer,
                    response.ReceiverPublicKeyHex);

                _channelOpeningMessageBuilder.BuildPairingPayload(response.Id, response.Version, senderHexPublicKey,
                    response.ReceiverRelayServer, senderAppName);

                _channelOpeningMessageBuilder.BuildEncryptedPayload(response.ReceiverPublicKeyHex);

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