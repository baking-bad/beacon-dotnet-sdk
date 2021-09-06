namespace BeaconSdk.Infrastructure.Transport.Communication
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Domain.Beacon.P2P;
    using MatrixSdk.Application;
    using MatrixSdk.Application.Listener;
    using MatrixSdk.Domain.Room;
    using MatrixSdk.Utils;
    using Sodium;

    public class Client
    {
        private readonly string appName;
        private KeyPair? keyPair;
        private readonly List<MatrixClient> matrixClients;
        private static readonly Dictionary<HexString, MatrixEventListener<List<BaseRoomEvent>>> EncryptedMessageListeners = new ();
       
        public Client(List<MatrixClient> matrixClients, string appName)
        {
            this.matrixClients = matrixClients;
            this.appName = appName;

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
        
        public async Task SendPairingResponse(BeaconPeer peer)
        {
            try
            {
                var pairingResponseAggregate = ClientMessageFactory.CreatePairingResponse(peer, keyPair!, appName);

                foreach (var matrixClient in matrixClients)
                    _ = await matrixClient.SendMessageAsync(pairingResponseAggregate.RecipientId, pairingResponseAggregate.ChannelOpeningMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}


// var (roomId, senderUserId, message) = textMessageEvent;
// // Todo: valida
            
// if (listenerId.Value != senderUserId)
//     Console.WriteLine($"RoomId: {roomId} received message from {senderUserId}: {message}.");