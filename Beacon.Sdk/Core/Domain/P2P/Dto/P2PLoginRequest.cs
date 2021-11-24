namespace Beacon.Sdk.Core.Domain.P2P.Dto
{
    using System;

    public record P2PLoginRequest(Uri Address, string Username, string Password, string DeviceId);
}