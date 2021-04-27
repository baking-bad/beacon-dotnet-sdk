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

    public record MatrixSyncRequestParams (int? Timeout = null, string? Since = null);


    public record TimeLine(List<MatrixStateEvent> Events);

    public record State(List<MatrixStateEvent> Events);

    public record InviteState(List<MatrixStateEvent> Events);


    public record JoinedRoom(TimeLine TimeLine, State State);

    public record InvitedRoom(InviteState InviteState);

    public record LeftRoom(TimeLine TimeLine, State State);


    public record Rooms(
        Dictionary<string, JoinedRoom> Join,
        Dictionary<string, InvitedRoom> Invite,
        Dictionary<string, LeftRoom> Leave);

    public record MatrixSyncResponse(string NextBatch, Rooms Rooms);
}