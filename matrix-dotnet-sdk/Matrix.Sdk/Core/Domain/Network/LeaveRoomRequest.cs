namespace Matrix.Sdk.Core.Domain.Network
{
    using System;

    public record LeaveRoomRequest(Uri NodeAddress, string AccessToken, string RoomId);
}