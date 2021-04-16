namespace BeaconSdk.ConsoleApp
{
    using System;
    using System.Threading.Tasks;
    using MatrixSdk.Extensions;
    using MatrixSdk.Services;
    using Microsoft.Extensions.DependencyInjection;

    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            Console.WriteLine("Init Dependencies");
            var serviceProvider = new ServiceCollection()
                .AddMatrixSdk()
                .AddConsoleApp()
                .BuildServiceProvider();

            var matrixUserService = serviceProvider.GetService<MatrixUserService>();
            var matrixRoomService = serviceProvider.GetService<MatrixRoomService>();

            var seed = Guid.NewGuid().ToString(); //Todo: generate once and then store seed?

            var responseLogin = await matrixUserService!.LoginAsync(seed);
            var responseCreateRoom = await matrixRoomService!.CreateRoomAsync(responseLogin.AccessToken, "");
            var responseJoinedRooms = await matrixRoomService.GetJoinedRoomsAsync(responseLogin.AccessToken);

            Console.WriteLine($"RoomId: {responseCreateRoom.RoomId}");

            foreach (var room in responseJoinedRooms.joinedRooms)
                Console.WriteLine($"Joined room: {room}");
        }
    }
}