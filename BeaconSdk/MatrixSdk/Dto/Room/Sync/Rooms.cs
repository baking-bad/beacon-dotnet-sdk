namespace MatrixSdk.Dto.Room.Sync
{
    using System.Collections.Generic;

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