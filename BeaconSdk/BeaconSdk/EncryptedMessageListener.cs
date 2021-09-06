namespace BeaconSdk
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Infrastructure.Cryptography;
    using Infrastructure.Repositories;
    using MatrixSdk.Application.Listener;
    using MatrixSdk.Domain.Room;
    using MatrixSdk.Infrastructure.Services;
    using MatrixSdk.Utils;
    using Sodium;

    public class EncryptedMessageListener : MatrixEventListener<List<BaseRoomEvent>>
    {
        public EncryptedMessageListener(KeyPair keyPair, HexString publicKeyToListen, Action<TextMessageEvent> onNewTextMessage)
        {
            this.keyPair = keyPair;
            this.publicKeyToListen = publicKeyToListen;
            this.onNewTextMessage = onNewTextMessage;
        }

        private readonly KeyPair keyPair;
        private readonly HexString publicKeyToListen;
        private readonly Action<TextMessageEvent> onNewTextMessage;
        
        public override void OnCompleted() => throw new NotImplementedException();

        public override void OnError(Exception error) => throw new NotImplementedException();
        
        public override void OnNext(List<BaseRoomEvent> value)
        {
            foreach (var matrixRoomEvent in value)
                if (matrixRoomEvent is TextMessageEvent textMessageEvent)
                    if (SenderIdMatchesPublicKeyToListen(textMessageEvent.SenderUserId, publicKeyToListen) && 
                        EncryptionService.Validate(textMessageEvent.Message)) // Todo: implement validate
                    {
                        var serverSessionKeyPair = SessionKeyPairInMemory.CreateOrReadServer(publicKeyToListen, keyPair);
                        var message = EncryptionServiceHelper.DecryptAsString(textMessageEvent.Message, serverSessionKeyPair.Rx);
                        
                        onNewTextMessage(textMessageEvent);
                    }
        }
        
        private bool SenderIdMatchesPublicKeyToListen(string senderUserId, HexString publicKey)
        {
            var hash = EncryptionService.Hash(publicKey.ToByteArray());

            return HexString.TryParse(hash, out var hexHash) && 
                   senderUserId.StartsWith($"@{hexHash}");
        }
        
        // private string Decrypt(string encryptedMessage, HexString publicKey)
        // {
        //     var encryptedBytes = HexString.TryParse(encryptedMessage, out var hexString)
        //         ? hexString.ToByteArray()
        //         : Encoding.UTF8.GetBytes(encryptedMessage);
        //
        //     var serverSessionKeyPair = SessionKeyPairInMemory.CreateOrReadServer(publicKey, keyPair);
        //     
        //     var decryptedBytes = EncryptionService.Decrypt(encryptedBytes, serverSessionKeyPair.Rx);
        //
        //     return Encoding.UTF8.GetString(decryptedBytes);
        // }
    }
}