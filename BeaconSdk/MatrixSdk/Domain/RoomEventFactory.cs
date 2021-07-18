namespace MatrixSdk.Domain
{

    // public enum MatrixRoomEventType
    // {
    //     Create,
    //     Join,
    //     Message
    // }

    // public class RoomEventFactory
    // {
    //     public BaseRoomEvent CreateFrom(RoomEvent roomEvent, string roomId) => roomEvent.EventType switch
    //     {
    //         // EventType.Unknown => null,
    //         EventType.Create => BuildCreateRoomEvent(roomEvent, roomId),
    //         EventType.Member => BuildRoomMemberEvent(roomEvent, roomId),
    //         // EventType.Message => null,
    //         _ => throw new ArgumentOutOfRangeException()
    //     };
    //
    //     private CreateRoomEvent BuildCreateRoomEvent(RoomEvent roomEvent, string roomId)
    //     {
    //         var content = roomEvent.Content.ToObject<RoomCreateContent>() ?? 
    //                       throw new InvalidOperationException($"Cannot parse {nameof(RoomCreateContent)}");
    //         
    //         return new CreateRoomEvent(
    //             roomEvent.EventId, 
    //             roomId, 
    //             roomEvent.OriginServerTimestamp,
    //             content.CreatorUserId);
    //     }
    //
    //     public BaseRoomEvent BuildRoomMemberEvent(RoomEvent roomEvent, string roomId)
    //     {
    //         var content = roomEvent.Content.ToObject<RoomMemberContent>() ??
    //                       throw new InvalidOperationException($"Cannot parse {nameof(RoomMemberContent)}");
    //
    //
    //         if (content.UserMembershipState == UserMembershipState.Join)
    //         {
    //             
    //         }
    //         else
    //         {
    //             
    //         }
    //         return null;
    //     }
    //     
    // }
}