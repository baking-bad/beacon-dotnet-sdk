namespace MatrixSdk.Application
{
    using System;
    using Domain.Room;

    public class TextMessageObserver : IObserver<TextMessageEvent>
    {
        private IDisposable cancellation;

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(TextMessageEvent value)
        {
            Console.WriteLine($"RoomId: {value.RoomId} received message from {value.SenderUserId}: {value.Message}");
        }

        public void Subscribe(TextMessageNotifier notifier)
        {
            cancellation = notifier.Subscribe(this);
        }

        public void Unsubscribe() => cancellation.Dispose();
    }
}