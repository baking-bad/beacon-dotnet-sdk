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
            var matrixEventService = serviceProvider.GetService<MatrixEventService>();

            var seed = Guid.NewGuid().ToString(); //Todo: generate once and then store seed?
            var responseLogin = await matrixUserService!.LoginAsync(seed);

            var accessToken = responseLogin.AccessToken;

            var responseCreateRoom = await matrixRoomService!.CreateRoomAsync(accessToken, "");
            var responseJoinedRooms = await matrixRoomService.GetJoinedRoomsAsync(accessToken);

            Console.WriteLine($"RoomId: {responseCreateRoom.RoomId}");

            foreach (var room in responseJoinedRooms.JoinedRooms)
                Console.WriteLine($"Joined room: {room}");

            var t = await matrixEventService!.SyncAsync(accessToken);
        }
    }
}