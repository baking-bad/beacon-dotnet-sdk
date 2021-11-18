namespace Beacon.Sdk.Core.Domain.Interfaces.Data
{
    using Infrastructure.Cryptography.Libsodium;
    using Sodium;
    using Utils;

    public interface IKeyPairRepository
    {
        SessionKeyPair CreateOrReadServerSession(HexString clientPublicKey, KeyPair serverKeyPair);
        
        KeyPair KeyPair { get; }
    }
}