namespace MatrixSdk.Dto.Room.Joined
{
    using System.Collections.Generic;

    public record JoinedRoomsResponse(List<string> JoinedRooms)
    {
        public List<string> JoinedRooms { get; } = JoinedRooms;
    }
}