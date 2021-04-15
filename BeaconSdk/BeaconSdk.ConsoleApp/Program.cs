namespace BeaconSdk.ConsoleApp
{
    using System;
    using System.Threading.Tasks;
    using MatrixSdk;
    using Microsoft.Extensions.DependencyInjection;

    internal class Program
    {
        private static async Task Main(string[] args)
        {
            Console.WriteLine("Init Dependencies");
            var serviceProvider = new ServiceCollection()
                .AddMatrixSdk()
                .BuildServiceProvider();

            var cryptoService = new CryptoService(new LibsodiumAlgorithmsProvider());

            // See: https://github.com/airgap-it/beacon-node/blob/master/docker/crypto_auth_provider.py
            var loginDigest = cryptoService.GenerateLoginDigest();
            var guid = Guid.NewGuid();
            var keyPair = cryptoService.GenerateKeyPairFromSeed(guid.ToString());
            var hexSignature = cryptoService.GenerateHexSignature(loginDigest, keyPair.PrivateKey);
            var hexPublicKey = Convert.ToHexString(keyPair.PublicKey).ToLower();

            var hexId = cryptoService.GenerateHexId(keyPair.PublicKey);
            var password = $"ed:{hexSignature}:{hexPublicKey}";
            var deviceId = hexPublicKey;

            var matrixClientService = serviceProvider.GetService<MatrixClientService>();
            var responseLogin = await matrixClientService.LoginAsync(hexId, password, deviceId);

            var member = "";
            var responseCreateRoom =
                await matrixClientService.CreateRoomAsync(responseLogin.AccessToken, null);
        }
    }
}