namespace Matrix.Sdk.Core.Domain.Services
{
    using System;
    using Sodium;

    public record LoginRequest(Uri NodeAddress, KeyPair KeyPair);
}