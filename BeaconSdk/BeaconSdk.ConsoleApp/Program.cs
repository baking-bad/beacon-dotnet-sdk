namespace BeaconSdk.ConsoleApp
{
    using System;
    using System.Threading.Tasks;
    using MatrixSdk;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Serilog;

    internal class Program
    {
        private static IHostBuilder CreateHostBuilder() => new HostBuilder()
            .ConfigureServices((hostContext, services) =>
            {
                services.AddMatrixSdk();
                services.AddConsoleApp();
            }).UseConsoleLifetime();

        private static async Task<int> Main(string[] args)
        {
            var host = CreateHostBuilder().Build();

            var theme = LoggerSetup.SetupTheme();
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console(theme: theme)
                .CreateLogger();

            try
            {
                var logger = host.Services.GetRequiredService<ILogger<Program>>();

                logger.LogInformation("START");

                await RunAsync(host.Services);
            }
            catch (Exception ex)
            {
                var logger = host.Services.GetRequiredService<ILogger<Program>>();

                logger.LogError(ex, "An error occurred.");
            }

            return 0;
        }

        private static async Task RunAsync(IServiceProvider serviceProvider)
        {
            var firstClient = serviceProvider.GetRequiredService<MatrixClient>();
            var secondClient = serviceProvider.GetRequiredService<MatrixClient>();

            // await firstClient!.StartAsync(Guid.NewGuid().ToString()); //Todo: generate once and then store seed?
            // await secondClient!.StartAsync(Guid.NewGuid().ToString());

            await firstClient!.StartAsync("10000"); //Todo: generate once and then store seed?
            await secondClient!.StartAsync("20000");

            // var room = await firstClient.CreateTrustedPrivateRoomAsync();
            // await firstClient.SendMessageAsync(room.Id, "Test");
            // await firstClient.SendMessageAsync(room.Id, "Test2");

            var firstClientMatrixRoom = await firstClient.CreateTrustedPrivateRoomAsync(new[]
            {
                secondClient.UserId
            });

            await secondClient.JoinTrustedPrivateRoomAsync(firstClientMatrixRoom.Id);

            // await firstClient.LeaveRoomAsync(firstClientMatrixRoom.Id);

            // var secondClientMatrixRoom = await secondClient.JoinTrustedPrivateRoomAsync(firstClientMatrixRoom.Id);

            // await secondClient.SendMessageAsync(secondClientMatrixRoom.Id, "Hello world!");
            // await firstClient.SendMessageAsync(firstClientMatrixRoom)
            Console.ReadLine();

            firstClient.Stop();
            // secondClient.Stop();
        }
    }
}

// var cts = new CancellationTokenSource();

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

// await firstClient.CreateTrustedPrivateRoomAsync(new[] {secondClient.UserId});
// await firstClient.CreateTrustedPrivateRoomAsync(new[] {secondClient.UserId});


// var joinedRooms = await firstClient.GetJoinedRoomsAsync();
// foreach (var room in joinedRooms)
//     Console.WriteLine($"Room: {room}");