namespace Matrix.Sdk.Core.Domain.Network
{
    using System;

    public record CreateTrustedPrivateRoomRequest(Uri BaseAddress, string AccessToken, string[] InvitedUserIds);
}