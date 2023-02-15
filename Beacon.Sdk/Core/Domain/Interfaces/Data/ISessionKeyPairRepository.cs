using Beacon.Sdk.Core.Infrastructure.Cryptography;

namespace Beacon.Sdk.Core.Domain.Interfaces.Data
{
    using Utils;

    public interface ISessionKeyPairRepository
    {
        SessionKeyPair CreateOrReadClient(HexString clientHexPublicKey, KeyPair serverKeyPair);

        SessionKeyPair CreateOrReadServer(HexString clientHexPublicKey, KeyPair serverKeyPair);
    }
}