namespace MatrixSdk.Application
{
    using System;
    using System.Collections.Generic;

    internal class Unsubscriber<T> : IDisposable
    {
        private readonly IObserver<T> observer;
        private readonly List<IObserver<T>> observers;

        public Unsubscriber(List<IObserver<T>> observers, IObserver<T> observer)
        {
            this.observers = observers;
            this.observer = observer;
        }

        public void Dispose()
        {
            if (observers.Contains(observer))
                observers.Remove(observer);
        }
    }
}