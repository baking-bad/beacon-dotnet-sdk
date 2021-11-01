namespace Matrix.Sdk.Core.Domain.Network
{
    using System;

    public record LeaveRoomRequest(Uri BaseAddress, string AccessToken, string RoomId);
}