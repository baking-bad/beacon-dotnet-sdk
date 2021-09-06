namespace BeaconSdk.Infrastructure.Transport.Communication
{
    using System;
    using Cryptography;
    using Domain.Beacon.P2P;
    using Domain.Pairing;
    using MatrixSdk.Utils;
    using Newtonsoft.Json;
    using Sodium;

    public static class ClientMessageFactory
    {
        
        public static PairingResponseAggregate CreatePairingResponse(BeaconPeer peer, KeyPair keyPair, string appName)
        {
            if (!HexString.TryParse(peer.PublicKey, out var publicKeyHex))
                throw new InvalidOperationException("Can not parse peer.PublicKey");

            var recipientId = CreateRecipientId(peer.RelayServer, publicKeyHex);
            var relayServer = string.Empty;
            var pairingPayloadMessage = CreatePairingPayload(peer, keyPair.PublicKey, relayServer, appName);

            var payload = EncryptionServiceHelper.EncryptAsHex(pairingPayloadMessage, publicKeyHex.ToByteArray()).ToString();
            var channelOpeningMessage = GetChannelOpeningMessage(recipientId, payload);

            return new PairingResponseAggregate(channelOpeningMessage, recipientId);
        }

        public readonly struct PairingResponseAggregate
        {
            public readonly string ChannelOpeningMessage;
            public readonly string RecipientId;
            
            public PairingResponseAggregate(string channelOpeningMessage, string recipientId)
            {
                ChannelOpeningMessage = channelOpeningMessage;
                RecipientId = recipientId;
            }
        }
        
        private static string GetChannelOpeningMessage(string recipient, string payload) => $"@channel-open:{recipient}:{payload}";
        
        private static string CreateRecipientId(string relayServer, HexString publicKey)
        {
            var bytesArray = publicKey.ToByteArray();
            var hash = GenericHash.Hash(bytesArray, null, bytesArray.Length)!;
            
            if (HexString.TryParse(hash, out var hexHash))
                return $"{hexHash}:{relayServer}";

            throw new InvalidOperationException("Can not parse hash");
        }

        private static string CreatePairingPayload(BeaconPeer peer, byte[] publicKey, string relayServer, string appName)
        {
            if (!int.TryParse(peer.Version, out var version))
                throw new InvalidOperationException("Invalid peer version");

            if (!HexString.TryParse(publicKey, out var hexPublicKey)) 
                throw new ArgumentException("Invalid publicKey");

            return version switch
            {
                1 => throw new InvalidOperationException("Only PairingPayloadV2 is supported"),
                2 => CreatePairingPayloadV2(peer, hexPublicKey, relayServer, appName),
                _ => CreatePairingPayloadV2(peer, hexPublicKey, relayServer, appName)
            };
        }

        private static string CreatePairingPayloadV1(HexString publicKey) => publicKey.ToString();

        private static string CreatePairingPayloadV2(BeaconPeer peer, HexString publicKey, string relayServer, string appName)
        {
            if (peer.Id == null)
                throw new ArgumentNullException(nameof(peer.Id));

            var pairingResponse = new PairingResponse(
                peer.Id, 
                "p2p-pairing-response",
                appName,
                peer.Version,
                publicKey.ToString(), 
                relayServer);

            return JsonConvert.SerializeObject(pairingResponse);
        }
    }
}