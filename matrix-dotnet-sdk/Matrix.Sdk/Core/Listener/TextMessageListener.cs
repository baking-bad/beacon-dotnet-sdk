namespace Matrix.Sdk.Core.Listener
{
    using System;
    using System.Collections.Generic;
    using Domain.Room;

    public class TextMessageListener : MatrixEventListener<List<BaseRoomEvent>>
    {
        private readonly string _listenerId;
        private readonly Action<string, TextMessageEvent> _onNewTextMessage;

        public TextMessageListener(string listenerId, Action<string, TextMessageEvent> onNewTextMessage)
        {
            _onNewTextMessage = onNewTextMessage;
            _listenerId = listenerId;
        }

        public override void OnCompleted() => throw new NotImplementedException();

        public override void OnError(Exception error) => throw new NotImplementedException();

        public override void OnNext(List<BaseRoomEvent> value)
        {
            foreach (BaseRoomEvent matrixRoomEvent in value)
                if (matrixRoomEvent is TextMessageEvent textMessageEvent)
                    _onNewTextMessage(_listenerId, textMessageEvent);
        }
    }
}
// private void OnNewTextMsg(string listenerId, TextMessageEvent value)
// {
//     var (roomId, senderUserId, message) = value;
//     if (listenerId != senderUserId)
//         Console.WriteLine($"RoomId: {roomId} received message from {senderUserId}: {message}.");
// }