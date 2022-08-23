namespace Beacon.Sdk.Core.Domain.Entities.P2P
{
    using System;
    using Interfaces;
    using Utils;

    public class P2PPeerRoomFactory
    {
        private readonly ICryptographyService _cryptographyService;

        public P2PPeerRoomFactory(ICryptographyService cryptographyService)
        {
            _cryptographyService = cryptographyService;
        }

        public P2PPeerRoom Create(string relayServer, HexString peerHexPublicKey, string peerName, string roomId)
        {
            byte[] hexBytes = peerHexPublicKey.ToByteArray();
            byte[] hash = _cryptographyService.Hash(hexBytes);

            if (!HexString.TryParse(hash, out HexString hexHash))
                throw new Exception();

            return new P2PPeerRoom
            {
                P2PUserId = $"@{hexHash}:{relayServer}",
                PeerHexPublicKey = peerHexPublicKey,
                RoomId = roomId,
                PeerName = peerName
            };
        }
    }
}