namespace BeaconSdk.ConsoleApp
{
    using System;
    using System.Threading.Tasks;
    using MatrixSdk;
    using Microsoft.Extensions.DependencyInjection;

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
        }
    }
}