namespace MatrixSdk.Dto.Room.Create
{
    public record CreateRoomResponse(string RoomId)
    {
        public string RoomId { get; } = RoomId;
    }
}