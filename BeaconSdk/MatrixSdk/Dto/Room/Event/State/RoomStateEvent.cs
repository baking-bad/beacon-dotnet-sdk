namespace MatrixSdk.Dto.Room.Event.State
{
    using Newtonsoft.Json.Linq;

    public record RoomStateEvent(string Sender, string EventId, RoomEventType Type, JObject Content)  : BaseRoomEvent(Type, Content)
    {
        public string Sender { get; } = Sender;
        public string EventId { get; } = EventId;
    }
}