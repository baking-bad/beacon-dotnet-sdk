namespace MatrixSdk.Application
{
    using System;
    using Domain.Room;

    public class TextMessageListener : IObserver<TextMessageEvent>
    {
        private readonly string userId;
        private IDisposable? cancellation;

        public TextMessageListener(string userId)
        {
            this.userId = userId;
        }

        public void OnCompleted() => throw new NotImplementedException();

        public void OnError(Exception error) => throw new NotImplementedException();

        public void OnNext(TextMessageEvent value)
        {
            if (userId != value.SenderUserId)
                Console.WriteLine($"RoomId: {value.RoomId} received message from {value.SenderUserId}: {value.Message}.");
        }

        public void ListenTo(TextMessageNotifier notifier) => cancellation = notifier.Subscribe(this);

        public void Unsubscribe() => cancellation?.Dispose();
    }
}