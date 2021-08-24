namespace BeaconSdk
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using MatrixSdk.Application;
    using MatrixSdk.Application.Listener;
    using MatrixSdk.Domain.Room;
    using Sodium;

    public class CommunicationClient
    {
        private readonly List<MatrixClient> matrixClients;
        private readonly Dictionary<string, TextMessageListener> textMessageListeners;
        public CommunicationClient(List<MatrixClient> matrixClients, Dictionary<string, TextMessageListener> textMessageListeners)
        {
            this.matrixClients = matrixClients;
            this.textMessageListeners = textMessageListeners;
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

        public void ListenToPublicKeyHex(string publicKeyHex)
        {
            if (textMessageListeners.TryGetValue(publicKeyHex, out _))
                return;

            // var id = Guid.NewGuid().ToString();
            // //listenerId -> listenToId
            // var listener = new TextMessageListener(id, publicKeyHex, OnNewEncryptedMessage);
            //
            // if (!textMessageListeners.TryAdd(publicKeyHex, listener))
            //     return;
            //
            // foreach (var client in matrixClients)
            //     listener.ListenTo(client.TextMessageNotifier);
        }

        private void OnNewEncryptedMessage(string listenerId, TextMessageEvent textMessageEvent)
        {
            var (roomId, senderUserId, message) = textMessageEvent;
            
            if (listenerId != senderUserId)
                Console.WriteLine($"RoomId: {roomId} received message from {senderUserId}: {message}.");
        }
        

        public void RemoveListenerForPublicKeyHex(string publicKeyHex)
        {
            if (textMessageListeners.TryGetValue(publicKeyHex, out var listener))
                listener.Unsubscribe();
        }

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