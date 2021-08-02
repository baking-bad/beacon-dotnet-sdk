namespace MatrixSdk.Infrastructure
{
    public static class MatrixApiConstants
    {
        public const string Matrix = nameof(Matrix);

        // public const string BaseAddress = "https://matrix.papers.tech/";
        public const string BaseAddress = "https://beacon-node-1.sky.papers.tech/";

        public class EventType
        {
            public const string Create = "m.room.create";

            public const string Member = "m.room.member";

            public const string Message = "m.room.message";
        }

        public class MessageType
        {
            // public const string Text = "m.text";
            public const string Text = "Text";
        }
    }
}