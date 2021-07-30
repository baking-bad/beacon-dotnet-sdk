namespace MatrixSdk.Domain.Room
{
    public abstract record BaseRoomEvent(string RoomId, string SenderUserId);
}