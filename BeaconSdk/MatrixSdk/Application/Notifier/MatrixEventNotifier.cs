namespace MatrixSdk.Application.Notifier
{
    using System;
    using System.Collections.Generic;

    public class MatrixEventNotifier<T> : IObservable<T>
    {
        private readonly List<IObserver<T>> observers = new();

        public IDisposable? Subscribe(IObserver<T> observer)
        {
            if (!observers.Contains(observer))
                observers.Add(observer);

            return new Unsubscriber<T>(observers, observer);
        }

        public void NotifyAll(T matrixEvent)
        {
            foreach (var eventObserver in observers)
                eventObserver.OnNext(matrixEvent);
        }
    }
}