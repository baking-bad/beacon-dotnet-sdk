namespace BeaconSdk.ConsoleApp
{
    using System;
    using System.Threading.Tasks;
    using MatrixSdk;
    using Microsoft.Extensions.DependencyInjection;
    using Sodium;

    // ReSharper disable once IdentifierTypo
    public class LibsodiumAlgorithmsProvider : ICryptoAlgorithmsProvider
    {
        public byte[] GenerateRandomBytes(int count) => 
            SodiumCore.GetRandomBytes(count);

        public byte[] Hash(string message, int size) => 
            GenericHash.Hash(message,(byte[]) null, size);

        public byte[] Hash(byte[] message, int size) =>
            GenericHash.Hash(message,(byte[]) null, size);

        public KeyPair GenerateEd25519KeyPair(byte[] seed) =>
            PublicKeyAuth.GenerateKeyPair(seed);

        public byte[] SignDetached(byte[] message, byte[] key)
        {
            // PublicKeyAuth.VerifyDetached()
            return PublicKeyAuth.SignDetached(message, key);
        }
    }
    
    public interface ICryptoAlgorithmsProvider
    {
        byte[] GenerateRandomBytes(int count);

        byte[] Hash(string message, int size);
        
        byte[] Hash(byte[] message, int size);

        KeyPair GenerateEd25519KeyPair(byte[] seed);

        byte[] SignDetached(byte[] message, byte[] key);

    }

    internal class Program
    {
        // private const string userId = "";
        //
        // private const string password = "";
        //
        // private const string deviceId = "";

        private static async Task Main(string[] args)
        {
            Console.WriteLine("Init Dependencies");
            var serviceProvider = new ServiceCollection()
                .AddMatrixSdk()
                .BuildServiceProvider();

            var cryptoService = new CryptoService(new LibsodiumAlgorithmsProvider());
            
            var loginDigest = cryptoService.GenerateLoginDigest();
            var guid = Guid.NewGuid();
            var keyPair = cryptoService.GenerateKeyPairFromSeed(guid.ToString());
            var hexSignature = cryptoService.GenerateHexSignature(loginDigest, keyPair.PrivateKey);
            var hexPublicKey = Convert.ToHexString(keyPair.PublicKey);
           
            var hexId = cryptoService.GenerateHexId(keyPair.PublicKey);
            var password = $"ed:{hexSignature}:{hexPublicKey}";
            var deviceId = hexPublicKey;
            
            var userService = serviceProvider.GetService<UserService>();
            var response = await userService.LoginAsync(hexId, password, deviceId); 
        }
        
        private class CryptoService
        {
            private readonly ICryptoAlgorithmsProvider cryptoAlgorithmsProvider;

            public CryptoService(ICryptoAlgorithmsProvider cryptoAlgorithmsProvider)
            {
                this.cryptoAlgorithmsProvider = cryptoAlgorithmsProvider;
            }

            public byte[] GenerateLoginDigest()
            {
                var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds() * 1000;
                var message = $"login:{now / 1000 / (5 * 60)}";
                
                return cryptoAlgorithmsProvider.Hash(message, 32);
            }

            public KeyPair GenerateKeyPairFromSeed(string seed)
            {
                var hash = cryptoAlgorithmsProvider.Hash(seed, 32);

                return cryptoAlgorithmsProvider.GenerateEd25519KeyPair(hash);
            }

            public string GenerateHexSignature(byte[] loginDigest, byte[] secretKey)
            {
                var signature = cryptoAlgorithmsProvider.SignDetached(loginDigest, secretKey);

                return Convert.ToHexString(signature);
            }

            public string GenerateHexId(byte[] publicKey)
            {
                var hash = cryptoAlgorithmsProvider.Hash(publicKey, publicKey.Length);

                return Convert.ToHexString(hash);
            }
        }
    }
}