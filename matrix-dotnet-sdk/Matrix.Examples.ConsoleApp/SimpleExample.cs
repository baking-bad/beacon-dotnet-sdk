namespace Matrix.Examples.ConsoleApp
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Sdk;
    using Sdk.Core.Domain.MatrixRoom;
    using Sdk.Core.Domain.Network;
    using Sdk.Core.Domain.Room;
    using Serilog;
    using Serilog.Sinks.SystemConsole.Themes;
    using Sodium;

    public class SimpleExample
    {
        private static readonly CryptographyService CryptographyService = new();

        private static LoginRequest CreateLoginRequest()
        {
            var seed = Guid.NewGuid().ToString();
            KeyPair keyPair = CryptographyService.GenerateEd25519KeyPair(seed);

            byte[] loginDigest = CryptographyService.GenerateLoginDigest();
            string hexSignature = CryptographyService.GenerateHexSignature(loginDigest, keyPair.PrivateKey);
            string publicKeyHex = CryptographyService.ToHexString(keyPair.PublicKey);
            string hexId = CryptographyService.GenerateHexId(keyPair.PublicKey);

            var password = $"ed:{hexSignature}:{publicKeyHex}";
            string deviceId = publicKeyHex;

            var baseAddress = new Uri("https://beacon-node-0.papers.tech:8448/");

            return new LoginRequest(baseAddress, hexId, password, deviceId);
        }

        public async Task Run()
        {
            SystemConsoleTheme theme = LoggerSetup.SetupTheme();
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console(theme: theme)
                .CreateLogger();

            var firstFactory = new MatrixClientFactory();
            var secondFactory = new MatrixClientFactory();

            IMatrixClient firstClient = firstFactory.Create();
            IMatrixClient secondClient = secondFactory.Create();

            firstClient.OnMatrixRoomEventsReceived += (sender, eventArgs) =>
            {
                foreach (BaseRoomEvent roomEvent in eventArgs.MatrixRoomEvents)
                {
                    if (roomEvent is not TextMessageEvent textMessageEvent)
                        continue;

                    (string roomId, string senderUserId, string message) = textMessageEvent;
                    if (firstClient.UserId != senderUserId)
                        Console.WriteLine($"RoomId: {roomId} received message from {senderUserId}: {message}.");
                }
            };

            secondClient.OnMatrixRoomEventsReceived += (sender, eventArgs) =>
            {
                foreach (BaseRoomEvent roomEvent in eventArgs.MatrixRoomEvents)
                {
                    if (roomEvent is not TextMessageEvent textMessageEvent)
                        continue;

                    (string roomId, string senderUserId, string message) = textMessageEvent;
                    if (firstClient.UserId != senderUserId)
                        Console.WriteLine($"RoomId: {roomId} received message from {senderUserId}: {message}.");
                }
            };

            await firstClient.LoginAsync(CreateLoginRequest());
            await secondClient.LoginAsync(CreateLoginRequest());

            MatrixRoom firstClientMatrixRoom = await firstClient.CreateTrustedPrivateRoomAsync(new[]
            {
                secondClient.UserId
            });

            MatrixRoom matrixRoom = await secondClient.JoinTrustedPrivateRoomAsync(firstClientMatrixRoom.Id);

            var spin = new SpinWait();
            while (secondClient.JoinedRooms.Length == 0)
                spin.SpinOnce();

            await firstClient.SendMessageAsync(firstClientMatrixRoom.Id, "Hello");
            await secondClient.SendMessageAsync(secondClient.JoinedRooms[0].Id, ", ");

            await firstClient.SendMessageAsync(firstClientMatrixRoom.Id, "World");
            await secondClient.SendMessageAsync(secondClient.JoinedRooms[0].Id, "!");


            Console.ReadLine();

            firstClient.Stop();
            secondClient.Stop();
        }
    }
}