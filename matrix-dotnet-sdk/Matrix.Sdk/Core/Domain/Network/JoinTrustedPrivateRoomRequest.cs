namespace Matrix.Sdk.Core.Domain.Network
{
    using System;

    public record JoinTrustedPrivateRoomRequest(Uri NodeAddress, string AccessToken, string RoomId);
}