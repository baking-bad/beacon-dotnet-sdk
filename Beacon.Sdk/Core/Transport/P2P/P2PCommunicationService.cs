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
    using Matrix.Sdk.Core.Domain.RoomEvent;
    using Utils;

    public class P2PCommunicationService : IP2PCommunicationService
    {
        public event EventHandler<P2PMessageEventArgs> OnP2PMessagesReceived;
        private readonly SessionCryptographyService _sessionCryptographyService;
        private readonly IChannelOpeningMessageBuilder _channelOpeningMessageBuilder;
        private readonly IMatrixClient _matrixClient;

        public P2PCommunicationService(
            SessionCryptographyService sessionCryptographyService,
            IMatrixClient matrixClient,
            IChannelOpeningMessageBuilder channelOpeningMessageBuilder)
        {
            _sessionCryptographyService = sessionCryptographyService;
            _matrixClient = matrixClient;
            _channelOpeningMessageBuilder = channelOpeningMessageBuilder;
        }

        public async Task LoginAsync()
        {
            SessionCryptographyService.LoginRequest request = await _sessionCryptographyService.CreateLoginRequest();
            
            await _matrixClient.LoginAsync(request.Address, request.Username, request.Password, request.DeviceId);
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
                string? message = _sessionCryptographyService.TryDecryptMessageFromEvent(matrixRoomEvent);
                
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
                var senderRelayServer =
                    (_matrixClient.BaseAddress?.Host ?? throw new NullReferenceException("Provide P2PClient BaseAddress"));

                _channelOpeningMessageBuilder.Reset();
                _channelOpeningMessageBuilder.BuildRecipientId(receiverRelayServer, receiverHexPublicKey);
                _channelOpeningMessageBuilder.BuildPairingPayload(id, version, "beacon-node-0.papers.tech:8448", appName);
                
                var t =_channelOpeningMessageBuilder.Message;
                
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

        public Task SendMessageAsync() => throw new NotImplementedException();
    }
}