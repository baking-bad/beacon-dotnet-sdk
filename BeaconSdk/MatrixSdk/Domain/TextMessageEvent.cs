namespace MatrixSdk.Domain
{
    public record TextMessageEvent : BaseRoomEvent
    {

        public TextMessageEvent(string eventId, string roomId, long originServerTimestamp, string senderUserId) : base(roomId, senderUserId)
        {
        }
    }
}