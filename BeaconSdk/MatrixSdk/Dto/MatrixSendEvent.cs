namespace MatrixSdk.Dto
{
    // Todo: rename msgtype
    public record MessageEvent(string msgtype, string messageType = "m.text");

    public record EventResponse(string? EventId);
}