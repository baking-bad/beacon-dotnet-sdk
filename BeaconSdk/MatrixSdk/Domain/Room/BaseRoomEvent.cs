namespace MatrixSdk.Domain.Room
{
    public abstract record BaseRoomEvent
    {
        public BaseRoomEvent(string roomId, string senderUserId)
        {
            RoomId = roomId;
            SenderUserId = senderUserId;
        }
        public string RoomId { get; }

        public string SenderUserId { get; }
    }
}