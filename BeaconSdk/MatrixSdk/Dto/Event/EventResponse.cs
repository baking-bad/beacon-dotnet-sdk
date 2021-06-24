namespace MatrixSdk.Dto.Event
{
    public record EventResponse(string? EventId)
    {
        public string? EventId { get; } = EventId;
    }
}