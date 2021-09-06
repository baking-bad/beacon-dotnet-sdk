namespace MatrixSdk.Application.Listener
{
    using System;
    using Notifier;

    public abstract class MatrixEventListener<T> : IObserver<T>
    {
        // ReSharper disable once MemberCanBePrivate.Global
        public readonly string Id;
        private IDisposable? cancellation;

        protected MatrixEventListener()
        {
            Id = Guid.NewGuid().ToString();
        }

        public abstract void OnCompleted();

        public abstract void OnError(Exception error);

        public abstract void OnNext(T value);

        public void ListenTo(MatrixEventNotifier<T> notifier) => cancellation = notifier.Subscribe(this);

        public void Unsubscribe() => cancellation?.Dispose();
    }
}