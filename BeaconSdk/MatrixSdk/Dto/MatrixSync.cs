namespace MatrixSdk.Dto
{
    /*
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
    using System.Collections.Generic;
    using Room.Event.State;
    

    public record MatrixSyncRequestParams (int? Timeout = null, string? Since = null)
    {
        public int? Timeout { get; } = Timeout;
        public string? Since { get; } = Since;
    }


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

    public record MatrixSyncResponse(string NextBatch, Rooms Rooms)
    {
        public string NextBatch { get; } = NextBatch;
        public Rooms Rooms { get; } = Rooms;
    }
}

 // public class Content
    // {
    //     public List<object> actions { get; set; }
    //     public string pattern { get; set; }
    //     public string rule_id { get; set; }
    //     public bool @default { get; set; }
    //     public bool enabled { get; set; }
    //     public Global global { get; set; }
    //     public Device device { get; set; }
    //     public string room_version { get; set; }
    //     public string creator { get; set; }
    //     public string membership { get; set; }
    //     public string displayname { get; set; }
    //     public object avatar_url { get; set; }
    //     public Users users { get; set; }
    //
    //     public int? users_default { get; set; }
    //
    //     // public Events events { get; set; }
    //     public int? events_default { get; set; }
    //     public int? state_default { get; set; }
    //     public int? ban { get; set; }
    //     public int? kick { get; set; }
    //     public int? redact { get; set; }
    //     public int? invite { get; set; }
    //     public string join_rule { get; set; }
    //     public string history_visibility { get; set; }
    //     public string guest_access { get; set; }
    // }
    //
    // public class Content3
    // {
    //     public string avatar_url { get; set; }
    //     public int last_active_ago { get; set; }
    //     public string presence { get; set; }
    //     public bool currently_active { get; set; }
    //     public string status_msg { get; set; }
    //     public string custom_config_key { get; set; }
    //     public string membership { get; set; }
    //     public string displayname { get; set; }
    //     public string body { get; set; }
    //     public string msgtype { get; set; }
    //     public string format { get; set; }
    //     public string formatted_body { get; set; }
    //     public List<string> user_ids { get; set; }
    //     // public Tags tags { get; set; }
    //     public string name { get; set; }
    // }