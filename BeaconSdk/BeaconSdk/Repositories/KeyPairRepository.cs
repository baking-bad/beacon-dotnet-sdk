namespace BeaconSdk.Repositories
{
    using System.Collections.Generic;
    using MatrixSdk.Infrastructure.Services;
    using MatrixSdk.Utils;
    using Sodium;

    public class KeyPairRepository
    {
        private readonly Dictionary<HexString, KeyPair> serverSessionKeyPairs = new();
        private readonly Dictionary<HexString, KeyPair> clientSessionKeyPairs = new();
        private readonly CryptoService cryptoService;
        
        public KeyPairRepository(CryptoService cryptoService)
        {
            this.cryptoService = cryptoService;
        }
        
        public KeyPair CreateOrReadServerSession(HexString publicKey)
        {
            if (serverSessionKeyPairs.TryGetValue(publicKey, out var result))
                return result;
            else
            {
                
            }
        }
    }
}