namespace Matrix.Sdk.Core.Domain.Network
{
    using System;

    public record LoginRequest(Uri NodeAddress, string User, string Password, string DeviceId);
}