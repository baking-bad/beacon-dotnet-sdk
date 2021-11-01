namespace Matrix.Sdk.Core.Domain.Network
{
    using System;

    public record LoginRequest(Uri BaseAddress, string User, string Password, string DeviceId);
}