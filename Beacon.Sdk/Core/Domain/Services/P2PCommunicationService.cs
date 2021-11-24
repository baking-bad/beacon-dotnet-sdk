namespace Beacon.Sdk.Core.Domain.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Interfaces;
    using P2P;
    using P2P.ChannelOpening;
    using Matrix.Sdk;
    using Matrix.Sdk.Core.Domain.MatrixRoom;
    using Matrix.Sdk.Core.Domain.RoomEvent;
    using P2P.Dto;
    
    
    public class P2PCommunicationService : IP2PCommunicationService
    {
        private readonly IChannelOpeningMessageBuilder _channelOpeningMessageBuilder;
        private readonly IMatrixClient _matrixClient;
        private readonly P2PMessageService _p2PMessageService;
        private readonly P2PLoginRequestFactory _p2PLoginRequestFactory;
        
        public P2PCommunicationService(
            P2PMessageService p2PMessageService,
            IMatrixClient matrixClient,
            IChannelOpeningMessageBuilder channelOpeningMessageBuilder,
            P2PLoginRequestFactory p2PLoginRequestFactory)
        {
            _p2PMessageService = p2PMessageService;
            _matrixClient = matrixClient;
            _channelOpeningMessageBuilder = channelOpeningMessageBuilder;
            _p2PLoginRequestFactory = p2PLoginRequestFactory;
        }

        public event EventHandler<P2PMessageEventArgs> OnP2PMessagesReceived;

        public async Task LoginAsync()
        {
            P2PLoginRequest request = await _p2PLoginRequestFactory.Create();

            await _matrixClient.LoginAsync(request.Address, request.Username, request.Password, request.DeviceId);
        }

        public void Start()
        {
            _matrixClient.OnMatrixRoomEventsReceived += OnMatrixRoomEventsReceived;
            _matrixClient.Start();
        }

        public void Stop()
        {
            _matrixClient.Stop();
            _matrixClient.OnMatrixRoomEventsReceived -= OnMatrixRoomEventsReceived;
        }

        public async Task<BeaconPeerRoom> SendChannelOpeningMessageAsync(string id, BeaconPeer beaconPeer, string appName)
        {
            try
            {
                string senderRelayServer =
                    _matrixClient.BaseAddress?.Host ??
                    throw new NullReferenceException("Provide P2PClient BaseAddress");

                _channelOpeningMessageBuilder.Reset();
                _channelOpeningMessageBuilder.BuildRecipientId(beaconPeer.RelayServer, beaconPeer.HexPublicKey);
                _channelOpeningMessageBuilder.BuildPairingPayload(id, beaconPeer.Version, "beacon-node-0.papers.tech:8448",
                    appName);

                ChannelOpeningMessage t = _channelOpeningMessageBuilder.Message;

                _channelOpeningMessageBuilder.BuildEncryptedPayload(beaconPeer.HexPublicKey);

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

                return new BeaconPeerRoom
                {
                    BeaconPeerHexPublicKey = beaconPeer.HexPublicKey,
                    RoomId = matrixRoom.Id
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task SendMessageAsync(string roomId, string message)
        {
            _ = await _matrixClient.SendMessageAsync(roomId, message);
        }

        private void OnMatrixRoomEventsReceived(object? sender, MatrixRoomEventsEventArgs e)
        {
            if (sender is not IMatrixClient)
                throw new ArgumentException("sender is not IMatrixClient");

            List<BaseRoomEvent> matrixRoomEvents = e.MatrixRoomEvents;
            var messages = new List<string>();

            foreach (BaseRoomEvent matrixRoomEvent in matrixRoomEvents)
            {
                string? message = _p2PMessageService.TryDecryptMessageFromEvent(matrixRoomEvent);

                if (message != null)
                    messages.Add(message);
            }

            OnP2PMessagesReceived.Invoke(this, new P2PMessageEventArgs(messages));
        }
    }
}


// public async Task<BeaconPeerRoom> SendChannelOpeningMessageAsync(string id, HexString receiverHexPublicKey,
//     string receiverRelayServer, string version, string appName)
// {
//     try
//     {
//         string senderRelayServer =
//             _matrixClient.BaseAddress?.Host ??
//             throw new NullReferenceException("Provide P2PClient BaseAddress");
//
//         _channelOpeningMessageBuilder.Reset();
//         _channelOpeningMessageBuilder.BuildRecipientId(receiverRelayServer, receiverHexPublicKey);
//         _channelOpeningMessageBuilder.BuildPairingPayload(id, version, "beacon-node-0.papers.tech:8448",
//             appName);
//
//         ChannelOpeningMessage t = _channelOpeningMessageBuilder.Message;
//
//         _channelOpeningMessageBuilder.BuildEncryptedPayload(receiverHexPublicKey);
//
//         ChannelOpeningMessage channelOpeningMessage = _channelOpeningMessageBuilder.Message;
//
//         // We force room creation here because if we "re-pair", we need to make sure that we don't send it to an old room.
//         MatrixRoom matrixRoom = await _matrixClient.CreateTrustedPrivateRoomAsync(new[]
//         {
//             channelOpeningMessage.RecipientId
//         });
//
//         var spin = new SpinWait();
//         while (_matrixClient.JoinedRooms.Length == 0)
//             spin.SpinOnce();
//         
//         _ = await _matrixClient.SendMessageAsync(matrixRoom.Id, channelOpeningMessage.ToString());
//         
//         return new BeaconPeerRoom
//         {
//             BeaconPeerHexPublicKey = receiverHexPublicKey;
//         }
//     }
//     catch (Exception ex)
//     {
//         Console.WriteLine(ex.Message);
//         throw;
//     }
// }