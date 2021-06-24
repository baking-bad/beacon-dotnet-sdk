namespace MatrixSdk.Dto.Room.Sync
{
    using System.Collections.Generic;
    using Event.State;

    public record State(List<RoomStateEvent> Events)
    {
        public List<RoomStateEvent> Events { get; } = Events;
    }
}