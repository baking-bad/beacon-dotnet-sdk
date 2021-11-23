namespace Beacon.Sdk.Core.Domain.Interfaces.Data
{
    using Infrastructure.Cryptography.Libsodium;
    using Utils;
    using Sodium;

    public interface ISessionKeyPairRepository
    {
        SessionKeyPair CreateOrReadClient(HexString clientHexPublicKey, KeyPair serverKeyPair);
        
        SessionKeyPair CreateOrReadServer(HexString clientHexPublicKey, KeyPair serverKeyPair);
    }
}