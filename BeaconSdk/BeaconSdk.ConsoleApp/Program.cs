namespace BeaconSdk.ConsoleApp
{
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using MatrixSdk;
    using Microsoft.Extensions.DependencyInjection;

    public class SodiumCryptoService : ICryptoService
    {
        public byte[] GenerateRandomBytes(long length) => throw new NotImplementedException();

        public byte[] Hash(byte[] message, long size) => throw new NotImplementedException();
    }
    public interface ICryptoService
    {
        byte[] GenerateRandomBytes(long length);

        byte[] Hash(byte[] message, long size);
    }

    internal class Program
    {
        private const string userId = "";

        private const string password = "";

        private const string deviceId = "";

        private static async Task Main(string[] args)
        {
            Console.WriteLine("Init Dependencies");
            var serviceProvider = new ServiceCollection()
                .AddMatrixSdk()
                .BuildServiceProvider();

            var userService = serviceProvider.GetService<UserService>();
            var response = await userService.LoginAsync(userId, password, deviceId);

            var cryptoService = new SodiumCryptoService();

            var time = DateTimeOffset.UtcNow.ToUnixTimeSeconds() * 1000;
            var message = $"login:{time / 1000 / (5 * 60)}";
            byte[] bytes = Encoding.UTF8.GetBytes(message);
            
            
            var loginDigest = cryptoService.Hash(message, 32);
        }
    }
}