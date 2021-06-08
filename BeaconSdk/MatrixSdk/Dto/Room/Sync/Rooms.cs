namespace MatrixSdk.Dto.Room.Sync
{
    using System.Collections.Generic;
    using Event.State; /*
           Naming rule: outer record - starts with 'Matrix'.
           Inner record - avoid 'Matrix'.
           
           For Example: 
           
           1. MatrixSyncResponse(string NextBatch, Rooms Rooms); - Outer record, starts with 'Matrix'.
           
           BUT
           
           2. public record Rooms(
                   Dictionary<string, JoinedRoom> Join,
                   Dictionary<string, InvitedRoom> Invite,
                   Dictionary<string, LeftRoom> Leave); 
                   
               a) 'Rooms' inner record for 'MatrixSyncResponse', so avoid 'Matrix'.
               b) 'JoinedRoom' inner record for 'Rooms' (and 'MatrixSyncResponse'), so avoid 'Matrix'.
               c) 'InvitedRoom' and 'LeftRoom' same as a) b)
       */


    public record TimeLine(List<RoomStateEvent> Events)
    {
        public List<RoomStateEvent> Events { get; } = Events;
    }

    public record State(List<RoomStateEvent> Events)
    {
        public List<RoomStateEvent> Events { get; } = Events;
    }

    public record InviteState(List<RoomStateEvent> Events)
    {
        public List<RoomStateEvent> Events { get; } = Events;
    }

    public record JoinedRoom(TimeLine TimeLine, State State)
    {
        public TimeLine TimeLine { get; } = TimeLine;
        public State State { get; } = State;
    }

    public record InvitedRoom(InviteState InviteState)
    {
        public InviteState InviteState { get; } = InviteState;
    }

    public record LeftRoom(TimeLine TimeLine, State State)
    {
        public TimeLine TimeLine { get; } = TimeLine;
        public State State { get; } = State;
    }
    public record Rooms(
        Dictionary<string, JoinedRoom> Join,
        Dictionary<string, InvitedRoom> Invite,
        Dictionary<string, LeftRoom> Leave)
    {
        public Dictionary<string, JoinedRoom> Join { get; } = Join;
        public Dictionary<string, InvitedRoom> Invite { get; } = Invite;
        public Dictionary<string, LeftRoom> Leave { get; } = Leave;
    }
}