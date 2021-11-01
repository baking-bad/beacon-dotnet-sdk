namespace Matrix.Sdk.Core.Domain.Network
{
    using System;

    public record GetJoinedRoomsIdsRequest(Uri BaseAddress, string AccessToken);
}