namespace Matrix.Sdk.Listener
{
    using System;
    using System.Collections.Generic;
    using Core.Domain.Room;

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