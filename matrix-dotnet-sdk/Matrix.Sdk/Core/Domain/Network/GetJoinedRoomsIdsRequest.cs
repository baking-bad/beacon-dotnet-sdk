namespace Matrix.Sdk.Core.Domain.Network
{
    using System;

    public record GetJoinedRoomsIdsRequest(Uri NodeAddress, string AccessToken);
}