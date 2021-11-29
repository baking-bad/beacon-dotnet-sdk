namespace Beacon.Sdk.Core.Domain.Entities.P2P
{
    using System;

    public record P2PLoginRequest(Uri Address, string Username, string Password, string DeviceId);
}