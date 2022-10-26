namespace Beacon.Sdk.Core.Domain.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Beacon;
    using Entities;
    using Entities.P2P;
    using Interfaces;
    using Interfaces.Data;
    using Matrix.Sdk;
    using Matrix.Sdk.Core.Domain.MatrixRoom;
    using Matrix.Sdk.Core.Domain.RoomEvent;
    using Matrix.Sdk.Core.Infrastructure.Dto.Room.Create;
    using Microsoft.Extensions.Logging;
    using P2P;
    using P2P.ChannelOpening;
    using P2P.Dto.Handshake;
    using Utils;
    using Constants = Constants;

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
        private readonly IMatrixSyncRepository _matrixSyncRepository;
        private readonly IJsonSerializerService _jsonSerializerService;
        private readonly PeerFactory _peerFactory;
        private readonly IPeerRepository _peerRepository;

        public P2PCommunicationService(
            ILogger<P2PCommunicationService> logger,
            IMatrixClient matrixClient,
            IChannelOpeningMessageBuilder channelOpeningMessageBuilder,
            IP2PPeerRoomRepository p2PPeerRoomRepository,
            IMatrixSyncRepository matrixSyncRepository,
            ICryptographyService cryptographyService,
            IJsonSerializerService jsonSerializerService,
            IPeerRepository peerRepository,
            P2PLoginRequestFactory p2PLoginRequestFactory,
            P2PPeerRoomFactory p2PPeerRoomFactory,
            P2PMessageService p2PMessageService,
            PeerFactory peerFactory)
        {
            _logger = logger;
            _matrixClient = matrixClient;
            _channelOpeningMessageBuilder = channelOpeningMessageBuilder;
            _p2PPeerRoomRepository = p2PPeerRoomRepository;
            _matrixSyncRepository = matrixSyncRepository;
            _cryptographyService = cryptographyService;
            _p2PLoginRequestFactory = p2PLoginRequestFactory;
            _p2PPeerRoomFactory = p2PPeerRoomFactory;
            _p2PMessageService = p2PMessageService;
            _jsonSerializerService = jsonSerializerService;
            _peerFactory = peerFactory;
            _peerRepository = peerRepository;
        }

        public event TaskEventHandler<P2PMessageEventArgs> OnP2PMessagesReceived;

        public bool LoggedIn { get; private set; }

        public bool Syncing { get; private set; }

        public async Task LoginAsync(string[] knownRelayServers)
        {
            P2PLoginRequest request = await _p2PLoginRequestFactory.Create(knownRelayServers);

            await _matrixClient.LoginAsync(request.Address, request.Username, request.Password, request.DeviceId);

            LoggedIn = true;
        }

        public void Start()
        {
            _matrixClient.OnMatrixRoomEventsReceived += OnMatrixRoomEventsReceived;

            MatrixSyncEntity? matrixSyncEntity = _matrixSyncRepository.TryReadAsync().Result;

            if (matrixSyncEntity != null)
                _matrixClient.Start(matrixSyncEntity.NextBatch);
            else
                _matrixClient.Start();

            Syncing = _matrixClient.IsSyncing;
        }

        public void Stop()
        {
            _matrixClient.Stop();
            _matrixClient.OnMatrixRoomEventsReceived -= OnMatrixRoomEventsReceived;

            Syncing = _matrixClient.IsSyncing;
        }

        public async Task<P2PPeerRoom> SendChannelOpeningMessageAsync(Peer peer, string id, string appName,
            string? appUrl, string? appIcon)
        {
            string senderRelayServer =
                _matrixClient.BaseAddress?.Host ??
                throw new NullReferenceException("Provide P2PClient BaseAddress");

            _channelOpeningMessageBuilder.Reset();
            _channelOpeningMessageBuilder.BuildRecipientId(peer.RelayServer, peer.HexPublicKey);
            _channelOpeningMessageBuilder.BuildPairingPayload(id, peer.Version, senderRelayServer, appName, appUrl,
                appIcon);
            _channelOpeningMessageBuilder.BuildEncryptedPayload(peer.HexPublicKey);

            ChannelOpeningMessage channelOpeningMessage = _channelOpeningMessageBuilder.Message;

            // We force room creation here because if we "re-pair", we need to make sure that we don't send it to an old room.
            CreateRoomResponse createRoomResponse = await _matrixClient.CreateTrustedPrivateRoomAsync(new[]
            {
                channelOpeningMessage.RecipientId
            });

            var spin = new SpinWait();

            MatrixRoom? needRoom = _matrixClient.JoinedRooms.FirstOrDefault(x => x.Id == createRoomResponse.RoomId);

            var wait = true;
            while (wait)
            {
                spin.SpinOnce();

                needRoom = _matrixClient.JoinedRooms.FirstOrDefault(x => x.Id == createRoomResponse.RoomId);

                if (needRoom != null)
                    if (needRoom.JoinedUserIds.Count == 2)
                        wait = false;
            }

            await _matrixClient.SendMessageAsync(createRoomResponse.RoomId, channelOpeningMessage.ToString());

            var p2PPeerRoom =
                _p2PPeerRoomFactory.Create(peer.RelayServer, peer.HexPublicKey, peer.Name, createRoomResponse.RoomId);
            await _p2PPeerRoomRepository.CreateOrUpdateAsync(p2PPeerRoom);

            var allDatabaseRoomIds = _p2PPeerRoomRepository.GetAll().Result.Select(room => room.RoomId);
            var abandonedRoomIds = (from room in _matrixClient.JoinedRooms
                where !allDatabaseRoomIds.Contains(room.Id)
                select room.Id).ToList();

            foreach (var abandonedRoomId in abandonedRoomIds)
            {
                await _matrixClient.LeaveRoomAsync(abandonedRoomId);
            }

            return p2PPeerRoom;
        }

        public async Task<P2PPairingRequest> GetPairingRequestInfo(string appName,
            string[] knownRelayServers,
            string? iconUrl,
            string? appUrl)
        {
            return new P2PPairingRequest(
                Id: KeyPairService.CreateGuid(),
                Name: appName,
                Version: Constants.BeaconVersion,
                PublicKey: _p2PLoginRequestFactory.GetPublicKeyHex(),
                RelayServer: await _p2PLoginRequestFactory.GetRelayServer(knownRelayServers),
                AppUrl: appUrl,
                Icon: iconUrl);
        }

        public async Task SendMessageAsync(Peer peer, string message)
        {
            var p2PPeerRoom = _p2PPeerRoomRepository.TryReadAsync(peer.HexPublicKey).Result
                              ?? throw new NullReferenceException(nameof(P2PPeerRoom));

            var encryptedMessage = _p2PMessageService.EncryptMessage(peer.HexPublicKey, message).Value;

            _ = await _matrixClient.SendMessageAsync(p2PPeerRoom.RoomId, encryptedMessage);

            // ToDo: Handle room not exist.
        }

        public async Task DeleteAsync(Peer peer)
        {
            var p2PeerRoom = _p2PPeerRoomRepository.TryReadAsync(peer.HexPublicKey).Result;
            if (p2PeerRoom != null)
                try
                {
                    await _matrixClient.LeaveRoomAsync(p2PeerRoom.RoomId);
                }
                catch (Exception e)
                {
                    _logger.LogError("{@Sender} Error during leaving room {@RoomId}", "Beacon", p2PeerRoom.RoomId);
                }

            await _p2PPeerRoomRepository.Remove(peer.HexPublicKey);
        }

        private string? TryGetMessageFromEvent(BaseRoomEvent matrixRoomEvent)
        {
            if (matrixRoomEvent is not TextMessageEvent textMessageEvent)
                return null;

            if (!HexString.TryParse(_p2PLoginRequestFactory.GetPublicKeyHex(), out var pubKeyHexString))
                throw new ArgumentException(nameof(pubKeyHexString));

            _channelOpeningMessageBuilder.Reset();
            _channelOpeningMessageBuilder.BuildRecipientId(_matrixClient.BaseAddress!.Host, pubKeyHexString);

            // check that sender is not ourselves
            if (textMessageEvent.SenderUserId == _channelOpeningMessageBuilder.Message.RecipientId)
                return null;

            if (textMessageEvent.Message.StartsWith(ChannelOpeningMessage.StartPrefix))
            {
                var payload = textMessageEvent.Message.Split(':')[^1];

                if (!_cryptographyService.Validate(payload))
                {
                    _logger.LogInformation("Can not validate payload");
                    return null;
                }

                if (!HexString.TryParse(_p2PLoginRequestFactory.GetPrivateKeyHex(), out HexString privateKeyHexString))
                    throw new ArgumentException(nameof(privateKeyHexString));

                var decrypted = _cryptographyService
                    .DecryptMessageAsString(payload, privateKeyHexString, pubKeyHexString);

                var pairingResponse = _jsonSerializerService.Deserialize<P2PPairingResponse>(decrypted);

                foreach (var room in _matrixClient.JoinedRooms)
                {
                    var foundUserId = room
                        .JoinedUserIds
                        .FirstOrDefault(userId => userId == textMessageEvent.SenderUserId);

                    if (foundUserId == null) continue;

                    if (!HexString.TryParse(pairingResponse.PublicKey, out HexString peerHexPublicKey))
                        throw new ArgumentException(nameof(peerHexPublicKey));

                    var peerRelayServer = foundUserId.Split(':').Last();
                    
                    var peer = _peerFactory.Create(
                        peerHexPublicKey,
                        pairingResponse.Name,
                        pairingResponse.Version,
                        peerRelayServer,
                        isActive: true
                    );

                    var p2PPeerRoom = _p2PPeerRoomFactory.Create(
                        peerRelayServer,
                        peerHexPublicKey,
                        pairingResponse.Name,
                        room.Id);

                    _ = Task.Run(async () =>
                    {
                        await _peerRepository.MarkAllInactive();
                        await _peerRepository.CreateAsync(peer);
                        await _p2PPeerRoomRepository.CreateOrUpdateAsync(p2PPeerRoom);
                        _ = OnP2PMessagesReceived.Invoke(this, new P2PMessageEventArgs(null, pairingResponse));
                    });
                    break;
                }
                
                return null;
            }

            var senderUserId = textMessageEvent.SenderUserId;
            var p2PPeerRoomFromDb = _p2PPeerRoomRepository.TryReadAsync(senderUserId).Result;

            if (p2PPeerRoomFromDb == null)
            {
                _logger.LogInformation("{@Sender}: Can't find P2PPeerRoom for {SenderId}",
                    "P2PCommunicationService", senderUserId);
                return null;
            }

            if (!_cryptographyService.Validate(textMessageEvent.Message))
            {
                _logger.LogInformation("Can not validate message");
                return null;
            }

            if (!HexString.TryParse(textMessageEvent.Message, out HexString hexMessage))
                throw new ArgumentException(nameof(textMessageEvent.Message));

            return _p2PMessageService.DecryptMessage(p2PPeerRoomFromDb.PeerHexPublicKey, hexMessage);
        }

        private async void OnMatrixRoomEventsReceived(object? sender, MatrixRoomEventsEventArgs e)
        {
            if (sender is not IMatrixClient)
                throw new ArgumentException("sender is not IMatrixClient");

            await _matrixSyncRepository.CreateOrUpdateAsync(e.NextBatch);

            var messages = new List<string>();

            foreach (BaseRoomEvent matrixRoomEvent in e.MatrixRoomEvents)
            {
                if (matrixRoomEvent is JoinRoomEvent joinRoomEvent)
                {
                    await _matrixClient.JoinTrustedPrivateRoomAsync(joinRoomEvent.RoomId);
                    return;
                }

                string? message = TryGetMessageFromEvent(matrixRoomEvent);

                if (message != null)
                    messages.Add(message);
            }

            _ = OnP2PMessagesReceived.Invoke(this, new P2PMessageEventArgs(messages));
        }
    }
}