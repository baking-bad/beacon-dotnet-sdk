namespace MatrixSdk.Dto.Room.Sync
{
    public record JoinedRoom(TimeLine TimeLine, State State)
    {
        public TimeLine TimeLine { get; } = TimeLine;
        public State State { get; } = State;
    }
}