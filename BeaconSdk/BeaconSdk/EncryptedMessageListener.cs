namespace BeaconSdk
{
    using System;
    using System.Collections.Generic;
    using MatrixSdk.Application.Listener;
    using MatrixSdk.Domain.Room;
    using MatrixSdk.Infrastructure.Services;
    using MatrixSdk.Utils;

    public class EncryptedMessageListener : MatrixEventListener<List<BaseRoomEvent>>
    {
        private readonly Action<TextMessageEvent> onNewTextMessage;
        private readonly CryptoService cryptoService;

        public EncryptedMessageListener(HexString publicKeyToListen, Action<TextMessageEvent> onNewTextMessage, CryptoService cryptoService)
        {
            this.publicKeyToListen = publicKeyToListen;
            this.onNewTextMessage = onNewTextMessage;
            this.cryptoService = cryptoService;
        }

        private readonly HexString publicKeyToListen;

        public override void OnCompleted() => throw new NotImplementedException();

        public override void OnError(Exception error) => throw new NotImplementedException();
        
        public override void OnNext(List<BaseRoomEvent> value)
        {
            foreach (var matrixRoomEvent in value)
                if (matrixRoomEvent is TextMessageEvent textMessageEvent)
                    //Todo: refactor cryptoService.Validate
                    if (SenderIdMatchesPublicKeyToListen(textMessageEvent.SenderUserId, publicKeyToListen) && cryptoService.Validate(textMessageEvent.Message))
                        onNewTextMessage(textMessageEvent);
        }

        private bool SenderIdMatchesPublicKeyToListen(string senderUserId, HexString publicKey)
        {
            var hash = cryptoService.Hash(publicKey.ToByteArray());

            return HexString.TryParse(hash, out var hexHash) && 
                   senderUserId.StartsWith($"@{hexHash}");
        }
    }
}