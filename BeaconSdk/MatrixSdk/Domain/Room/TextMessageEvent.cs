namespace MatrixSdk.Domain.Room
{
    public record TextMessageEvent : BaseRoomEvent
    {

        public TextMessageEvent(string roomId, string senderUserId) : base(roomId, senderUserId)
        {
        }
    }
}