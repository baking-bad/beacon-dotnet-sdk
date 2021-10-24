namespace Beacon.Sdk
{
    using System;
    using System.Collections.Generic;
    using Core.Infrastructure.Cryptography;
    using Core.Infrastructure.Cryptography.Libsodium;
    using Core.Infrastructure.Repositories;
    using Matrix.Sdk.Core.Domain.Room;
    using Matrix.Sdk.Core.Utils;
    using Matrix.Sdk.Listener;
    using Sodium;

    public class EncryptedMessageListener : MatrixEventListener<List<BaseRoomEvent>>
    {
        private readonly KeyPair _keyPair;
        private readonly Action<TextMessageEvent> _onNewTextMessage;
        private readonly HexString _publicKeyToListen;

        public EncryptedMessageListener(KeyPair keyPair, HexString publicKeyToListen,
            Action<TextMessageEvent> onNewTextMessage)
        {
            _keyPair = keyPair;
            _publicKeyToListen = publicKeyToListen;
            _onNewTextMessage = onNewTextMessage;
        }

        public override void OnCompleted() => throw new NotImplementedException();

        public override void OnError(Exception error) => throw new NotImplementedException();

        public override void OnNext(List<BaseRoomEvent> value)
        {
            foreach (BaseRoomEvent matrixRoomEvent in value)
                if (matrixRoomEvent is TextMessageEvent textMessageEvent)
                    if (SenderIdMatchesPublicKeyToListen(textMessageEvent.SenderUserId, _publicKeyToListen) &&
                        BeaconCryptographyService.Validate(textMessageEvent.Message)) // Todo: implement validate
                    {
                        SessionKeyPair serverSessionKeyPair =
                            SessionKeyPairInMemory.CreateOrReadServer(_publicKeyToListen, _keyPair);
                        string message =
                            BeaconCryptographyService.DecryptAsString(textMessageEvent.Message,
                                serverSessionKeyPair.Rx);

                        _onNewTextMessage(textMessageEvent);
                    }
        }

        private bool SenderIdMatchesPublicKeyToListen(string senderUserId, HexString publicKey)
        {
            byte[] hash = BeaconCryptographyService.Hash(publicKey.ToByteArray());

            return HexString.TryParse(hash, out HexString hexHash) &&
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