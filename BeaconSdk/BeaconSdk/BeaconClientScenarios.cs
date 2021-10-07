using MatrixSdk.Utils;

namespace BeaconSdk
{
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using Base58Check;
    using Domain.Beacon.P2P;
    using Domain.Pairing;
    using Infrastructure.Cryptography;
    using Infrastructure.Serialization;
    using Infrastructure.Transport.Communication;
    using MatrixSdk.Application;
    using MatrixSdk.Application.Listener;
    using MatrixSdk.Infrastructure.Services;
    using Microsoft.Extensions.DependencyInjection;
    using Newtonsoft.Json;

    public static class BeaconClientScenarios
    {
        // ReSharper disable once InconsistentNaming
        public static async Task Setup(IServiceProvider serviceProvider)
        {
            var QRCode =
                "3NDKTWt2x3L4HZ5aUay1Krdah1g1g2i4NJdy59zK6RASXQ2XquyAJTVavtqYPeHzysyZgv2h3xA43Xu7i2kEbmc3jGjp5k5KoZEG84peABdh2smnUKcxVzhFFmTc4PE22L2eHBMNtGkcXzES1pkWgn1PZuvRZf1jwT6yBStjYhMJFd7FjvGSV1P8FGd8ht3D37VWfikkXpBni5jANdG8PcW6uSunNq4vGYnYZwXqLYEaFGJ36fTSeA52sWCNE9nB6NkHcH3cgcXiRt6PvAUmV5Hn73PegFipDAo7TUU7WqLKpX7gMD3p4DFiWLKKzNQ1q4uJAt5";
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

            // var seed = Guid.NewGuid().ToString();
            var seed = "44cf34fa-b4d4-ec89-cb5c-22e14f6156c6";
            // var public = BeaconCryptographyService.ToHexString(Encoding.Default.GetBytes(seed));
            
            // BeaconCryptographyService.ToHexString(seed);
            var keyPair = BeaconCryptographyService.GenerateEd25519KeyPair(seed);

            var clientOptions = new ClientOptions("Test App Name");
            var matrixClient = serviceProvider.GetRequiredService<MatrixClient>();
            var jsonSerializerService = serviceProvider.GetRequiredService<JsonSerializerService>();
            
            var client = new Client(clientOptions, matrixClient, jsonSerializerService);

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

// var publicKey = BeaconCryptographyService.ToHexString(keyPair.PublicKey);
