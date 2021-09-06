namespace BeaconSdk.Communication
{
    using System;
    using System.Net.Http.Json;
    using Domain.Beacon;
    using Domain.Beacon.P2P;
    using Domain.Pairing;
    using MatrixSdk.Utils;
    using Newtonsoft.Json;
    using Sodium;

    internal  static class CommunicationClientUtils
    {
        public static string GetChannelOpeningMessage(string recipient, string payload) => $"@channel-open:{recipient}:{payload}";

        public static string CreateRecipientId(string relayServer, HexString publicKey)
        {
            var hash = Hash(publicKey);
            if (HexString.TryParse(hash, out var hexHash))
                return $"{hexHash}:{relayServer}";

            throw new InvalidOperationException("Can not parse hash");
        }

        private static byte[] Hash(HexString hexString)
        {
            var bytesArray = hexString.ToByteArray();

            return GenericHash.Hash(bytesArray, null, bytesArray.Length)!;
        }
        
        
        public static string CreatePairingPayload(BeaconPeer peer, byte[] publicKey, string relayServer, string appName)
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

// if (result == "null") throw new NullReferenceException(nameof(PairingResponse));
//
// return result;