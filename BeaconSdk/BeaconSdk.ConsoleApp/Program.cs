namespace BeaconSdk.ConsoleApp
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using MatrixSdk;
    using MatrixSdk.Extensions;
    using Microsoft.Extensions.DependencyInjection;
    using Serilog;

    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()
                .AddMatrixSdk()
                .AddConsoleApp()
                .BuildServiceProvider();

            var theme = LoggerSetup.SetupTheme();

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console(theme: theme)
                .CreateLogger();


            var cts = new CancellationTokenSource();


            // var matrixUserService = serviceProvider.GetService<MatrixUserService>();
            // var matrixRoomService = serviceProvider.GetService<MatrixRoomService>();
            // var matrixEventService = serviceProvider.GetService<MatrixEventService>();
            //
            // var seed = Guid.NewGuid().ToString(); //Todo: generate once and then store seed?
            // var responseLogin = await matrixUserService!.LoginAsync(seed);
            //
            // var accessToken = responseLogin.AccessToken;
            //
            // var responseCreateRoom = await matrixRoomService!.CreateRoomAsync(accessToken, "");
            // var responseJoinedRooms = await matrixRoomService.GetJoinedRoomsAsync(accessToken, cts.Token);
            //
            // Console.WriteLine($"RoomId: {responseCreateRoom.RoomId}");
            //
            // foreach (var room in responseJoinedRooms.JoinedRooms)
            //     Console.WriteLine($"Joined room: {room}");
            //
            // var t = await matrixEventService!.SyncAsync(accessToken, cts.Token);

            var firstClient = serviceProvider.GetService<MatrixClient>();
            var secondClient = serviceProvider.GetService<MatrixClient>();

            await firstClient!.StartAsync(Guid.NewGuid().ToString()); //Todo: generate once and then store seed?
            await secondClient!.StartAsync(Guid.NewGuid().ToString());
            var firstClientMatrixRoom = await firstClient.CreateTrustedPrivateRoomAsync(new[]
            {
                secondClient.UserId
            });

            var secondClientMatrixRoom = await secondClient.JoinTrustedPrivateRoomAsync(firstClientMatrixRoom.Id);

            // await firstClient.CreateTrustedPrivateRoomAsync(new[] {secondClient.UserId});
            // await firstClient.CreateTrustedPrivateRoomAsync(new[] {secondClient.UserId});


            // var joinedRooms = await firstClient.GetJoinedRoomsAsync();
            // foreach (var room in joinedRooms)
            //     Console.WriteLine($"Room: {room}");


            Console.ReadLine();
        }
    }
}