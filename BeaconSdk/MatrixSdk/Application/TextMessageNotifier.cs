namespace MatrixSdk.Application
{
    using System;
    using System.Collections.Generic;
    using Domain.Room;

    public class TextMessageNotifier : IObservable<TextMessageEvent>
    {
        private readonly List<IObserver<TextMessageEvent>> observers = new();

        public IDisposable Subscribe(IObserver<TextMessageEvent> observer)
        {
            if (!observers.Contains(observer))
                observers.Add(observer);

            return new Unsubscriber<TextMessageEvent>(observers, observer);
        }

        private void NotifyAll(TextMessageEvent messageEvent)
        {
            foreach (var eventObserver in observers)
                eventObserver.OnNext(messageEvent);
        }

        public void NotifyAll(List<BaseRoomEvent> baseRoomEvents)
        {
            foreach (var matrixRoomEvent in baseRoomEvents)
            {
                var textMessageEvent = matrixRoomEvent as TextMessageEvent;
                if (textMessageEvent != null)
                    NotifyAll(textMessageEvent);
            }
        }
    }

}