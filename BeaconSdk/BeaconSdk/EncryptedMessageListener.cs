namespace BeaconSdk
{
    using System;
    using System.Collections.Generic;
    using Infrastructure.Cryptography;
    using Infrastructure.Repositories;
    using MatrixSdk.Application.Listener;
    using MatrixSdk.Domain.Room;
    using MatrixSdk.Utils;
    using Sodium;

    public class EncryptedMessageListener : MatrixEventListener<List<BaseRoomEvent>>
    {

        private readonly KeyPair _keyPair;
        private readonly Action<TextMessageEvent> _onNewTextMessage;
        private readonly HexString _publicKeyToListen;
        public EncryptedMessageListener(KeyPair keyPair, HexString publicKeyToListen, Action<TextMessageEvent> onNewTextMessage)
        {
            _keyPair = keyPair;
            _publicKeyToListen = publicKeyToListen;
            _onNewTextMessage = onNewTextMessage;
        }

        public override void OnCompleted() => throw new NotImplementedException();

        public override void OnError(Exception error) => throw new NotImplementedException();

        public override void OnNext(List<BaseRoomEvent> value)
        {
            foreach (var matrixRoomEvent in value)
                if (matrixRoomEvent is TextMessageEvent textMessageEvent)
                    if (SenderIdMatchesPublicKeyToListen(textMessageEvent.SenderUserId, _publicKeyToListen) &&
                        BeaconCryptographyService.Validate(textMessageEvent.Message)) // Todo: implement validate
                    {
                        var serverSessionKeyPair = SessionKeyPairInMemory.CreateOrReadServer(_publicKeyToListen, _keyPair);
                        var message = BeaconCryptographyService.DecryptAsString(textMessageEvent.Message, serverSessionKeyPair.Rx);

                        _onNewTextMessage(textMessageEvent);
                    }
        }

        private bool SenderIdMatchesPublicKeyToListen(string senderUserId, HexString publicKey)
        {
            var hash = BeaconCryptographyService.Hash(publicKey.ToByteArray());

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