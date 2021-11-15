namespace Beacon.Sdk.Core.Domain.Interfaces.Data
{
    using Infrastructure.Cryptography.Libsodium;
    using Matrix.Sdk.Core.Utils;
    using Sodium;

    public interface IKeyPairRepository
    {
        SessionKeyPair CreateOrReadServerSession(HexString clientPublicKey, KeyPair serverKeyPair);
        
        KeyPair KeyPair { get; }
    }
}