namespace BeaconSdk
{
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using Base58Check;
    using Domain.Beacon.P2P;
    using Domain.Pairing;
    using Infrastructure.Cryptography;
    using Infrastructure.Transport.Communication;
    using MatrixSdk.Application;
    using MatrixSdk.Application.Listener;
    using MatrixSdk.Infrastructure.Services;
    using Microsoft.Extensions.DependencyInjection;
    using Newtonsoft.Json;

    public static class BeaconClientScenarios
    {
        // ReSharper disable once InconsistentNaming
        public static async Task Setup(MatrixClient matrixClient)
        {
            var QRCode =
                "BSdNU2tFbvqMLsJrysLuQPwY2ujnWVR47HviiJciUBzykFchn6LFSfWGRmXGEDVgMGjuWf8S4m3jUhf9Wnx88u7ja6NHyCwSRdyBxPb5izcp3JP8e8h8bYoxYt5ZRffERhkpPjaRSt8iDw6BiSJvzm2qVdaTL5jdARyx4cGBnxYPLtpCFWGHcKSYhbbaXmdbs1pZ9uKUoLx5WGR7y7gfdstC4kd3v9wP7S2nYkcTfb6BVBShRjdCNVNAEnvmyttEH5fwER3uAJX2GkWBifKmDqdinWis8fp4Vx2hdWmqReW4MkQWKKx9ChKHgAPjkwZXrmwposxq";
                
            var decodedBytes = Base58CheckEncoding.Decode(QRCode);
            var message = Encoding.Default.GetString(decodedBytes);

            var pairingResponse = JsonConvert.DeserializeObject<PairingResponse>(message);

            var beaconPeer = new BeaconPeer(
                pairingResponse!.Id,
                pairingResponse.Name,
                pairingResponse.PublicKey,
                pairingResponse.RelayServer,
                pairingResponse.Version,
                null,
                null);

            var seed = Guid.NewGuid().ToString();
            var keyPair = BeaconCryptographyService.GenerateEd25519KeyPair(seed);
            var client = new Client(matrixClient, "Test App Name");

            await client.StartAsync(keyPair);
            await client.SendPairingResponseAsync(beaconPeer);
            Console.ReadLine();
        }

        private static async Task<(MatrixClient, TextMessageListener)> SetupClientWithTextListener(IServiceProvider serviceProvider)
        {
            var matrixClient = serviceProvider.GetRequiredService<MatrixClient>();
            var seed = Guid.NewGuid().ToString();
            var keyPair = MatrixCryptographyService.GenerateEd25519KeyPair(seed);

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
    }
}