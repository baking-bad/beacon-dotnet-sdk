namespace Beacon.Sdk.Core.Domain.Interfaces.Data
{
    using Infrastructure.Cryptography.Libsodium;
    using Matrix.Sdk.Core.Utils;
    using Sodium;

    public interface ISessionKeyPairRepository
    {
        SessionKeyPair CreateOrReadServer(HexString clientPublicKey, KeyPair serverKeyPair);
    }
}