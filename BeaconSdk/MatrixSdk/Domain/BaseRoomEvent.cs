namespace MatrixSdk.Domain
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