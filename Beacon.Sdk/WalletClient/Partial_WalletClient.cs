namespace Beacon.Sdk.WalletClient
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Base58Check;
    using Beacon;
    using Core.Domain;
    using Core.Domain.Interfaces;
    using Core.Domain.P2P;
    using Utils;

    public partial class WalletClient
    {
        private string SenderId => Base58CheckEncoding.Encode(_cryptographyService.Hash(BeaconId.ToByteArray(), 5));

        private async Task SendAcknowledgeResponseAsync(BeaconBaseMessage beaconBaseMessage)
        {
            var acknowledgeResponse =
                new AcknowledgeResponse(Constants.BeaconVersion, beaconBaseMessage.Id, SenderId);

            Peer peer = _peerRepository.TryReadByUserId(beaconBaseMessage.SenderId).Result
                                    ?? throw new NullReferenceException(nameof(Peer));

            PeerRoom peerRoom = _peerRoomRepository.TryRead(peer.HexPublicKey).Result
                                            ?? throw new NullReferenceException(nameof(PeerRoom));

            
            string message = _jsonSerializerService.Serialize(acknowledgeResponse);

            await _p2PCommunicationService.SendMessageAsync(peer, peerRoom.RoomId, message);
        }

        private async Task OnP2PMessagesReceived(object? sender, P2PMessageEventArgs e)
        {
            if (sender is not IP2PCommunicationService)
                throw new ArgumentException("sender is not IP2PCommunicationClient");

            foreach (string message in e.Messages)
            {
                BeaconBaseMessage beaconBaseMessage = _jsonSerializerService.Deserialize<BeaconBaseMessage>(message);

                if (beaconBaseMessage.Version != "1")
                    await SendAcknowledgeResponseAsync(beaconBaseMessage);

                OnBeaconMessageReceived?.Invoke(this, new BeaconMessageEventArgs(beaconBaseMessage));
            }
        }
    }
}