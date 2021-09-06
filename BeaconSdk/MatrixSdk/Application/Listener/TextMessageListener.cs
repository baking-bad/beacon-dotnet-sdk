namespace MatrixSdk.Application.Listener
{
    using System;
    using System.Collections.Generic;
    using Domain.Room;

    public class TextMessageListener : MatrixEventListener<List<BaseRoomEvent>>
    {

        private readonly string listenerId;
        private readonly Action<string, TextMessageEvent> onNewTextMessage;

        public TextMessageListener(string listenerId, Action<string, TextMessageEvent> onNewTextMessage)
        {
            this.onNewTextMessage = onNewTextMessage;
            this.listenerId = listenerId;
        }

        public override void OnCompleted() => throw new NotImplementedException();

        public override void OnError(Exception error) => throw new NotImplementedException();

        public override void OnNext(List<BaseRoomEvent> value)
        {
            foreach (var matrixRoomEvent in value)
                if (matrixRoomEvent is TextMessageEvent textMessageEvent)
                    onNewTextMessage(listenerId, textMessageEvent);
        }
    }
}
// private void OnNewTextMsg(string listenerId, TextMessageEvent value)
// {
//     var (roomId, senderUserId, message) = value;
//     if (listenerId != senderUserId)
//         Console.WriteLine($"RoomId: {roomId} received message from {senderUserId}: {message}.");
// }