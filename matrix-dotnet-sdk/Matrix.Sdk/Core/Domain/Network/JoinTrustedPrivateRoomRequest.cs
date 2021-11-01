namespace Matrix.Sdk.Core.Domain.Network
{
    using System;

    public record JoinTrustedPrivateRoomRequest(Uri BaseAddress, string AccessToken, string RoomId);
}