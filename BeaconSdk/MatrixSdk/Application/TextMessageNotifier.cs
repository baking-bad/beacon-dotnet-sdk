namespace MatrixSdk.Application
{
    using System;
    using System.Collections.Generic;
    using Domain.Room;

    public class TextMessageNotifier : IObservable<TextMessageEvent>
    {
        private readonly List<IObserver<TextMessageEvent>> Observers = new();

        public IDisposable Subscribe(IObserver<TextMessageEvent> observer)
        {
            if (!Observers.Contains(observer))
                Observers.Add(observer);

            return new Unsubscriber<TextMessageEvent>(Observers, observer);
        }

        public void NotifyAll(TextMessageEvent messageEvent)
        {
            foreach (var eventObserver in Observers)
                eventObserver.OnNext(messageEvent);
        }
    }

}