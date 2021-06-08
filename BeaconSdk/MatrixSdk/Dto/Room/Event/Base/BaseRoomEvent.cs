namespace MatrixSdk.Dto.RoomEvent.BaseRoomEvent
{
    using Room.Event;

    public record BaseRoomEvent(RoomEventType Type, BaseRoomContent Content)
    {
        /// <summary>
        /// <b>Required.</b> The type of event.
        /// This SHOULD be namespaced similar to Java package naming conventions e.g. 'com.example.subdomain.event.type'
        /// </summary>
        public RoomEventType Type { get; init; } = Type;

        /// <summary>
        /// <b>Required.</b> The fields in this object will vary depending on the type of event.
        /// When interacting with the REST API, this is the HTTP body.
        /// </summary>
        public BaseRoomContent Content { get; init; } = Content;
    }
}