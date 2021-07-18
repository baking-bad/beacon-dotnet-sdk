namespace MatrixSdk.Domain
{
    public abstract record BaseRoomEvent
    {
        public BaseRoomEvent(string eventId, string roomId, long originServerTimestamp, string senderUserId)
        {
            EventId = eventId;
            RoomId = roomId;
            OriginServerTimestamp = originServerTimestamp;
            SenderUserId = senderUserId;
        }
        public string EventId { get; }

        public string RoomId { get; }

        public long OriginServerTimestamp { get; }

        public string SenderUserId { get; }
    }
}