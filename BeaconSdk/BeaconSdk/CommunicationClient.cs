namespace BeaconSdk
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Domain.Beacon;
    using MatrixSdk.Application;
    using MatrixSdk.Application.Listener;
    using MatrixSdk.Domain.Room;
    using MatrixSdk.Utils;
    using Sodium;

    public class CommunicationClient
    {
        private KeyPair? keyPair;
        private readonly List<MatrixClient> matrixClients;
        private static readonly Dictionary<HexString, MatrixEventListener<List<BaseRoomEvent>>> EncryptedMessageListeners = new ();
       
        public CommunicationClient(List<MatrixClient> matrixClients)
        {
            this.matrixClients = matrixClients;

            //Todo: refactor
            SodiumCore.Init();
        }

        // var seed = Guid.NewGuid().ToString();
        // var keyPair = cryptoService.GenerateKeyPairFromSeed(seed);
        public async Task StartAsync(KeyPair keyPair)
        {
            try
            {
                foreach (var client in matrixClients)
                    await client.StartAsync(keyPair);

                this.keyPair = keyPair;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void ListenToPublicKeyHex(HexString publicKey)
        {
            if (EncryptedMessageListeners.TryGetValue(publicKey, out _))
                return;

            // ReSharper disable once ArgumentsStyleNamedExpression
            var listener = new EncryptedMessageListener(keyPair!, publicKey, (e) =>
            {
                
            });

            EncryptedMessageListeners[publicKey] = listener;

            foreach (var client in matrixClients)
                listener.ListenTo(client.MatrixEventNotifier);
        }
        
        public void RemoveListenerForPublicKey(HexString publicKey)
        {
            if (EncryptedMessageListeners.TryGetValue(publicKey, out var listener))
                listener.Unsubscribe();
        }
        
        public void SendPairingResponse(BeaconPeer peer)
        {
            try
            {
                if (HexString.TryParse(peer.PublicKey, out var publicKeyHex))
                {
                    var recipientId = CommunicationClientUtils.CreateRecipientId(peer.RelayServer, publicKeyHex);
                    
                }

                throw new InvalidOperationException("Can not parse peer.Public");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }

    public static class CommunicationClientUtils
    {
        public static string CreateChannelOpeningMessage(string recipient, string payload) => $"@channel-open:{recipient}:{payload}";

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
        
        
        public static string CreatePairingPayload(BeaconPeer peer, byte[] publicKey)
        {
            
        }
        
    }
}

// var (roomId, senderUserId, message) = textMessageEvent;
// // Todo: valida
            
// if (listenerId.Value != senderUserId)
//     Console.WriteLine($"RoomId: {roomId} received message from {senderUserId}: {message}.");