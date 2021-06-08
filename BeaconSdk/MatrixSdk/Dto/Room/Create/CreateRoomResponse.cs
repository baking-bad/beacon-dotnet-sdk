namespace MatrixSdk.Dto.Room.Create
{
    public record CreateRoomResponse(string? RoomId = null)
    {
        public string? RoomId { get; } = RoomId;
    }
}