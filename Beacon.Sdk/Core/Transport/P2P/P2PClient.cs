namespace Beacon.Sdk.Core.Transport.P2P
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using ChannelOpening;
    using Dto.Handshake;
    using Matrix.Sdk;
    using Matrix.Sdk.Core.Domain.MatrixRoom;
    using Matrix.Sdk.Core.Domain.Room;
    using Matrix.Sdk.Core.Utils;
    using Matrix.Sdk.Listener;
    using Sodium;

    public class P2PClient : IP2PClient
    {
        private static readonly Dictionary<HexString, MatrixEventListener<List<BaseRoomEvent>>>
            EncryptedMessageListeners = new();

        private readonly IChannelOpeningMessageBuilder _channelOpeningMessageBuilder;
        private readonly IMatrixClient _matrixClient;

        private readonly RelayServerService _relayServerService;

        private string? _appName;
        private KeyPair? _keyPair;

        public P2PClient(IMatrixClient matrixClient, IChannelOpeningMessageBuilder channelOpeningMessageBuilder,
            RelayServerService relayServerService)
        {
            _matrixClient = matrixClient;
            _channelOpeningMessageBuilder = channelOpeningMessageBuilder;
            _relayServerService = relayServerService;

            //Todo: refactor
            SodiumCore.Init();
        }

        public async Task StartAsync(KeyPair keyPair)
        {
            try
            {
                string relayServer = await _relayServerService.GetRelayServer(keyPair.PublicKey);
                var nodeAddress = new Uri(relayServer);

                await _matrixClient.StartAsync(nodeAddress, keyPair);
                // _matrixClient.MatrixEventNotifier.
                // _matrixClient.StartSync();

                _keyPair = keyPair;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void ListenToPublicKeyHex(HexString publicKey, EncryptedMessageListener listener)
        {
            if (EncryptedMessageListeners.TryGetValue(publicKey, out _))
                return;

            // ReSharper disable once ArgumentsStyleNamedExpression
            // var listener = new EncryptedMessageListener(_cryptographyService, _keyPair!, publicKey, e => { });
            listener.ListenTo(_matrixClient.MatrixEventNotifier);

            EncryptedMessageListeners[publicKey] = listener;
        }

        public void RemoveListenerForPublicKey(HexString publicKey)
        {
            if (EncryptedMessageListeners.TryGetValue(publicKey, out MatrixEventListener<List<BaseRoomEvent>> listener))
                listener.Unsubscribe();
        }


        public async Task SendPairingResponseAsync(PairingResponse response)
        {
            try
            {
                if (!HexString.TryParse(_keyPair!.PublicKey, out HexString senderPublicKeyHex))
                    throw new InvalidOperationException("Can not parse sender public key.");

                _channelOpeningMessageBuilder.Reset();
                _channelOpeningMessageBuilder.BuildRecipientId(response.ReceiverRelayServer,
                    response.ReceiverPublicKeyHex);

                _channelOpeningMessageBuilder.BuildPairingPayload(response.Id, response.Version, senderPublicKeyHex,
                    response.ReceiverRelayServer, response.SenderAppName);

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