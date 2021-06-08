namespace MatrixSdk.Dto.Room.Event.State.Content
{
    using System.ComponentModel;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public record IsExternalInit;
    
    ///<remarks>
    ///m.room.join_rules
    ///</remarks> 
    public record RoomJoinRulesContent(RoomJoinRule RoomJoinRule)
    {
        /// <summary>
        /// <b>Required.</b> The type of rules used for users wishing to join this room.
        /// One of: ["public", "knock", "invite", "private"]
        /// </summary>
        public RoomJoinRule RoomJoinRule { get; } = RoomJoinRule;
    }
}