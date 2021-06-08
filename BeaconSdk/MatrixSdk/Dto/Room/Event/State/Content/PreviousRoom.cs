namespace MatrixSdk.Dto.Room.Event.State.Content
{
    public record PreviousRoom(string RoomId, string EventId)
    {
        /// <summary>
        /// <b>Required.</b> The ID of the old room.
        /// </summary>
        public string RoomId { get; } = RoomId;

        /// <summary>
        /// <b>Required.</b> The ID of the old room.
        /// </summary>
        public string EventId { get; } = EventId;
    }
}