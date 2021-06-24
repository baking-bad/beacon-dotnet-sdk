namespace MatrixSdk.Dto.Room.Sync
{
    public record InvitedRoom(InviteState InviteState)
    {
        public InviteState InviteState { get; } = InviteState;
    }
}