namespace Beacon.Sdk.Core.Domain.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Entities;
    using Entities.P2P;
    using Interfaces;
    using Interfaces.Data;
    using Matrix.Sdk;
    using Matrix.Sdk.Core.Domain.MatrixRoom;
    using Matrix.Sdk.Core.Domain.RoomEvent;
    using Microsoft.Extensions.Logging;
    using P2P;
    using P2P.ChannelOpening;
    using Utils;

    public class P2PCommunicationService : IP2PCommunicationService
    {
        private readonly IChannelOpeningMessageBuilder _channelOpeningMessageBuilder;
        private readonly ICryptographyService _cryptographyService;
        private readonly ILogger<P2PCommunicationService> _logger;
        private readonly IMatrixClient _matrixClient;
        private readonly P2PLoginRequestFactory _p2PLoginRequestFactory;
        private readonly P2PMessageService _p2PMessageService;
        private readonly P2PPeerRoomFactory _p2PPeerRoomFactory;
        private readonly IP2PPeerRoomRepository _p2PPeerRoomRepository;

        public P2PCommunicationService(
            ILogger<P2PCommunicationService> logger,
            IMatrixClient matrixClient,
            IChannelOpeningMessageBuilder channelOpeningMessageBuilder,
            IP2PPeerRoomRepository p2PPeerRoomRepository,
            ICryptographyService cryptographyService,
            P2PLoginRequestFactory p2PLoginRequestFactory,
            P2PPeerRoomFactory p2PPeerRoomFactory,
            P2PMessageService p2PMessageService)
        {
            _logger = logger;
            _matrixClient = matrixClient;
            _channelOpeningMessageBuilder = channelOpeningMessageBuilder;
            _p2PPeerRoomRepository = p2PPeerRoomRepository;
            _cryptographyService = cryptographyService;
            _p2PLoginRequestFactory = p2PLoginRequestFactory;
            _p2PPeerRoomFactory = p2PPeerRoomFactory;
            _p2PMessageService = p2PMessageService;
        }

        public event TaskEventHandler<P2PMessageEventArgs> OnP2PMessagesReceived;

        public bool LoggedIn { get; private set; }
        
        public bool Syncing { get; private set; }

        public async Task LoginAsync()
        {
            P2PLoginRequest request = await _p2PLoginRequestFactory.Create();

            await _matrixClient.LoginAsync(request.Address, request.Username, request.Password, request.DeviceId);

            LoggedIn = true;
        }

        public void Start()
        {
            _matrixClient.OnMatrixRoomEventsReceived += OnMatrixRoomEventsReceived;
            _matrixClient.Start();

            Syncing = _matrixClient.IsSyncing;
        }

        public void Stop()
        {
            _matrixClient.Stop();
            _matrixClient.OnMatrixRoomEventsReceived -= OnMatrixRoomEventsReceived;

            Syncing = _matrixClient.IsSyncing;
        }

        public async Task<P2PPeerRoom> SendChannelOpeningMessageAsync(Peer peer, string id, string appName)
        {
            string senderRelayServer =
                _matrixClient.BaseAddress?.Host ??
                throw new NullReferenceException("Provide P2PClient BaseAddress");

            _channelOpeningMessageBuilder.Reset();
            _channelOpeningMessageBuilder.BuildRecipientId(peer.RelayServer, peer.HexPublicKey);
            _channelOpeningMessageBuilder.BuildPairingPayload(id, peer.Version,
                "beacon-node-0.papers.tech:8448",
                appName);

            _channelOpeningMessageBuilder.BuildEncryptedPayload(peer.HexPublicKey);

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

            P2PPeerRoom p2PPeerRoom = _p2PPeerRoomFactory.Create(peer.RelayServer, peer.HexPublicKey, matrixRoom.Id);

            return _p2PPeerRoomRepository.CreateOrUpdate(p2PPeerRoom).Result;
        }

        public async Task SendMessageAsync(Peer peer, string message)
        {
            P2PPeerRoom p2PPeerRoom = _p2PPeerRoomRepository.TryRead(peer.HexPublicKey).Result
                                      ?? throw new NullReferenceException(nameof(P2PPeerRoom));

            string encryptedMessage = _p2PMessageService.EncryptMessage(peer.HexPublicKey, message).Value;

            _ = await _matrixClient.SendMessageAsync(p2PPeerRoom.RoomId, encryptedMessage);

            // ToDo: Handle room not exist.
        }

        private string? TryGetMessageFromEvent(BaseRoomEvent matrixRoomEvent)
        {
            if (matrixRoomEvent is not TextMessageEvent textMessageEvent) return null;

            string senderUserId = textMessageEvent.SenderUserId;
            P2PPeerRoom? p2PPeerRoom = _p2PPeerRoomRepository.TryRead(senderUserId).Result;

            if (p2PPeerRoom == null)
                // _logger.LogInformation("Unknown senderUserId");
                return null;

            if (!_cryptographyService.Validate(textMessageEvent.Message))
            {
                _logger.LogInformation("Can not validate message");
                return null;
            }

            if (!HexString.TryParse(textMessageEvent.Message, out HexString hexMessage))
                throw new ArgumentException(nameof(textMessageEvent.Message));

            return _p2PMessageService.DecryptMessage(p2PPeerRoom.PeerHexPublicKey, hexMessage);
        }

        private void OnMatrixRoomEventsReceived(object? sender, MatrixRoomEventsEventArgs e)
        {
            if (sender is not IMatrixClient)
                throw new ArgumentException("sender is not IMatrixClient");

            var messages = new List<string>();

            foreach (BaseRoomEvent matrixRoomEvent in e.MatrixRoomEvents)
            {
                string? message = TryGetMessageFromEvent(matrixRoomEvent);

                if (message != null)
                    messages.Add(message);
            }

            OnP2PMessagesReceived.Invoke(this, new P2PMessageEventArgs(messages));
        }
    }
}