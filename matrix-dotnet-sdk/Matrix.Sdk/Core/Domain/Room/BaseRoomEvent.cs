namespace Matrix.Sdk.Core.Domain.Room
{
    public abstract record BaseRoomEvent(string RoomId, string SenderUserId);
}