namespace BeaconSdk.ConsoleApp
{
    using System;
    using System.IO;
    using System.Reflection.Metadata;
    using System.Threading;
    using System.Threading.Tasks;
    using MatrixSdk;
    using MatrixSdk.Application;
    using MatrixSdk.Application.Listener;
    using MatrixSdk.Infrastructure.Providers;
    using MatrixSdk.Infrastructure.Services;
    using MatrixSdk.Utils;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Serilog;

    internal class Program
    {
        private static IHostBuilder CreateHostBuilder() => new HostBuilder()
            .ConfigureServices((hostContext, services) =>
            {
                services.AddBeaconSdk();
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
            // var a1 = "0xFAFA";
            // if (HexString.TryParse(a1, out var r1)) 
            //     Console.WriteLine(r1);
            //
            // var a2 = "0xFAFA";
            // if (HexString.TryParse(a2, out var r2)) 
            //     Console.WriteLine(r2);
            //
            // Console.WriteLine(r1 == r2);
            //
            //
            // var a3 = "0x38";
            // HexString.TryParse(a3, out var r3);
            // Console.WriteLine(r3.ToASCII());
            //
            // var b3 = r3.ToByteArray();
            // Console.WriteLine(r3.ToASCII());
            
           

            // var a3 = "48656c6c6f20576f726c6421";
            // if (HexString.TryParse(a3, out var r3)) 
            //     Console.WriteLine(r3.ToASCII());

            // var t = new HexString();
            // Console.WriteLine(t.ToASCII());нну

            // const string a3 = "0xFAF";
            // _ = HexString.TryParse(a3, out var r3);
            // Assert.AreEqual(r3.Value,string.Empty);

            // Object a = new Object();
            // var t = (HexString)a;

        }
        
        // private static async Task RunAsync(IServiceProvider serviceProvider)
        // {
        //     var (firstClient, firstListener) = await SetupClientWithTextListener(serviceProvider);
        //     var (secondClient, secondListener) = await SetupClientWithTextListener(serviceProvider);
        //
        //     var firstClientMatrixRoom = await firstClient.CreateTrustedPrivateRoomAsync(new[]
        //     {
        //         secondClient.UserId
        //     });
        //
        //     var matrixRoom = await secondClient.JoinTrustedPrivateRoomAsync(firstClientMatrixRoom.Id);
        //
        //     var spin = new SpinWait();
        //     while (secondClient.JoinedRooms.Length == 0)
        //         spin.SpinOnce();
        //
        //     await firstClient.SendMessageAsync(firstClientMatrixRoom.Id, "Hello");
        //     await secondClient.SendMessageAsync(secondClient.JoinedRooms[0].Id, ", ");
        //
        //     await firstClient.SendMessageAsync(firstClientMatrixRoom.Id, "World");
        //     await secondClient.SendMessageAsync(secondClient.JoinedRooms[0].Id, "!");
        //
        //     Console.ReadLine();
        //
        //     firstClient.Stop();
        //     secondClient.Stop();
        //
        //     firstListener.Unsubscribe();
        //     secondListener.Unsubscribe();
        // }

        private static async Task<(MatrixClient, TextMessageListener)> SetupClientWithTextListener(IServiceProvider serviceProvider)
        {
            var matrixClient = serviceProvider.GetRequiredService<MatrixClient>();
            var cryptoService = serviceProvider.GetRequiredService<CryptoService>();

            var seed = Guid.NewGuid().ToString();
            var keyPair = cryptoService.GenerateKeyPairFromSeed(seed);

            await matrixClient.StartAsync(keyPair); //Todo: generate once and then store seed?
            
            var textMessageListener = new TextMessageListener(matrixClient.UserId, (listenerId, textMessageEvent) =>
            {
                var (roomId, senderUserId, message) = textMessageEvent;
                if (listenerId != senderUserId)
                    Console.WriteLine($"RoomId: {roomId} received message from {senderUserId}: {message}.");
            });
            
            textMessageListener.ListenTo(matrixClient.MatrixEventNotifier);

            return (matrixClient, textMessageListener);
        }
        
        // public override void OnNext(TextMessageEvent value)
        // {
        //     var (roomId, senderUserId, message) = value;
        //     if (Id != senderUserId)
        //         Console.WriteLine($"RoomId: {roomId} received message from {senderUserId}: {message}.");
        // }
    }
}

// await firstClient!.StartAsync("077777"); 
// await secondClient!.StartAsync("08777");

// var room = await firstClient.CreateTrustedPrivateRoomAsync();
// await firstClient.SendMessageAsync(room.Id, "Test");
// await firstClient.SendMessageAsync(room.Id, "Test2");

// await firstClient.LeaveRoomAsync(firstClientMatrixRoom.Id);

// var secondClientMatrixRoom = await secondClient.JoinTrustedPrivateRoomAsync(firstClientMatrixRoom.Id);

// await secondClient.SendMessageAsync(secondClientMatrixRoom.Id, "Hello world!");
// await firstClient.SendMessageAsync(firstClientMatrixRoom)

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
// var cryptoService = new CryptoService(new LibsodiumAlgorithmsProvider());
// var input = new byte[]
// {
//     104,
//     101,
//     108,
//     108,
//     111,
//     104,
//     101,
//     108,
//     108,
//     111,
//     104,
//     101,
//     108,
//     108,
//     111,
//     104,
//     101,
//     108,
//     108,
//     111,
//     104,
//     101,
//     108,
//     108,
//     111,
// };
// foreach (var b in input)
//     Console.Write((char)b);
// Console.WriteLine();
//
// var hash = cryptoService.Hash(input);
// foreach (var b in hash)
//     Console.Write((char)b);
// Console.WriteLine();
//
// int.TryParse("null", out  int res);
// Console.WriteLine(res);