namespace BeaconSdk
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Threading.Tasks;
    using MatrixSdk;
    using MatrixSdk.Application;
    using MatrixSdk.Application.Listener;
    using MatrixSdk.Domain.Room;
    using MatrixSdk.Infrastructure.Services;
    using MatrixSdk.Utils;
    using Sodium;

    public class CommunicationClient
    {
        // private BetterHexString t;
        private readonly CryptoService cryptoService;
        private readonly List<MatrixClient> matrixClients;
        private readonly Dictionary<HexString, MatrixEventListener<List<BaseRoomEvent>>> encryptedMessageListeners;
       
        public CommunicationClient(List<MatrixClient> matrixClients, 
            Dictionary<HexString, MatrixEventListener<List<BaseRoomEvent>>> encryptedMessageListeners, CryptoService cryptoService)
        {
            this.matrixClients = matrixClients;
            this.encryptedMessageListeners = encryptedMessageListeners;
            this.cryptoService = cryptoService;
        }

        // var seed = Guid.NewGuid().ToString();
        // var keyPair = cryptoService.GenerateKeyPairFromSeed(seed);
        public async Task StartAsync(KeyPair keyPair)
        {
            try
            {
                foreach (var client in matrixClients)
                    await client.StartAsync(keyPair);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void ListenToPublicKeyHex(HexString publicKey)
        {
            if (encryptedMessageListeners.TryGetValue(publicKey, out _))
                return;

            // ReSharper disable once ArgumentsStyleNamedExpression
            var listener = new EncryptedMessageListener(publicKey, OnNewEncryptedMessage, cryptoService);

            encryptedMessageListeners[publicKey] = listener;

            foreach (var client in matrixClients)
                listener.ListenTo(client.MatrixEventNotifier);
        }

        private void OnNewEncryptedMessage(TextMessageEvent textMessageEvent)
        {
            // Todo: validate
            var (roomId, senderUserId, message) = textMessageEvent;
            
            // if (listenerId.Value != senderUserId)
            //     Console.WriteLine($"RoomId: {roomId} received message from {senderUserId}: {message}.");
        }

        // bool IsMessageFromPublicKey(TextMessageEvent textMessageEvent, HexString publicKeyHex)
        // {
        //     // var hexId = cryptoService.GenerateHexId(publicKeyHex);
        //     // return textMessageEvent.SenderUserId.StartsWith($"@{hexId}");
        // }
        

        public void RemoveListenerForPublicKey(HexString publicKey)
        {
            if (encryptedMessageListeners.TryGetValue(publicKey, out var listener))
                listener.Unsubscribe();
        }

        private static string CreateChannelOpeningMessage(string recipient, string payload) => $"@channel-open:{recipient}:{payload}";

        public void SendPairingResponse()
        {
            try
            {
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    } 
}