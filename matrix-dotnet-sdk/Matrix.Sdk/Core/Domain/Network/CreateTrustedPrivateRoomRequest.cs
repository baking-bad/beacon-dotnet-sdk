namespace Matrix.Sdk.Core.Domain.Network
{
    using System;

    public record CreateTrustedPrivateRoomRequest(Uri NodeAddress, string AccessToken, string[] InvitedUserIds);
}