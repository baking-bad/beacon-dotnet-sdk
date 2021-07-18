namespace MatrixSdk.Infrastructure.Dto.Event
{
    public record EventResponse(string? EventId)
    {
        public string? EventId { get; } = EventId;
    }
}